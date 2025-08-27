import * as github from "@actions/github";
import * as core from "@actions/core";

const { context } = github;
const { repo: repoInfo, payload, eventName } = context;
const owner = repoInfo.owner;
const repo = repoInfo.repo;
const token = process.env.GITHUB_TOKEN;
const BOT_LOGIN = "github-actions[bot]";
const octokit = github.getOctokit(token);

const GET_COMMENTS_QUERY = `
  query ($owner: String!, $repo: String!, $number: Int!, $last: Int!, $before: String) {
    repository(owner: $owner, name: $repo) {
      discussion(number: $number) {
        id
        number
        comments(last: $last, before: $before) {
          pageInfo{
            startCursor
            hasPreviousPage
          }
          nodes {
            id
            body
            author { login }
          }
        }
      }
    }
  }
`;
const UPDATE_DISCUSSION_QUERY = `
mutation($discussionId: ID!, $body: String){
  updateDiscussion(input: {
    discussionId: $discussionId,
    body: $body
  }){
    discussion{
      id,
      number,
      title,
      body,
      createdAt
    }
  }
}`;

function isBotSender(payload) {
  const login = payload?.sender?.login || "";
  const type = payload?.sender?.type || "";
  return type === "Bot" || login === BOT_LOGIN || login.endsWith("[bot]");
}

async function consumeComments(number, consumer) {
  let before = null;
  let comments = null;
  do {
    comments = await getComments(number, before);
    let commentNodes = comments?.repository?.discussion?.comments?.nodes;
    for (let i = 0; i < commentNodes.length; i++) {
      if (consumer(commentNodes[i])) return;
    }
    before = comments?.repository?.discussion?.comments?.pageInfo?.startCursor;
  } while (
    comments?.repository?.discussion?.comments?.pageInfo?.hasPreviousPage
  );
}

async function getComments(number, before, last = 20) {
  const variables = {
    owner,
    repo,
    number,
    last,
    before,
  };
  return await octokit.graphql(GET_COMMENTS_QUERY, variables);
}

async function updateDiscussion(discussionId, body) {
  const variables = {
    discussionId,
    body,
  };
  return await octokit.graphql(UPDATE_DISCUSSION_QUERY, variables);
}

async function OnEnterRoom() {
  let jsonObject = JSON.parse(payload.discussion.body);
  let observers = new Set(jsonObject.observer);
  observers.add(payload.sender.login);
  jsonObject.observers = observers;
  let json = JSON.stringify(jsonObject);
  await updateDiscussion(payload.discussion.node_id, json);
}

async function OnSelectFaction(faction) {
  let jsonObject = JSON.parse(payload.discussion.body);
  jsonObject.seats[faction] = payload.sender.login;
  let json = JSON.stringify(jsonObject);
  await updateDiscussion(payload.discussion.node_id, json);
}

async function run() {
  if (eventName == "discussion_comment") {
    if (payload.action !== "created") return;
    if (isBotSender(payload)) {
      core.info("来自 bot 的评论事件，忽略");
      return;
    }
    if (payload.comment?.body == "/enter") {
      await OnEnterRoom();
    } else if (payload.comment?.body.startsWith("/choose/faction")) {
      let faction = payload.comment?.body.split("/").pop();
      await OnSelectFaction(faction);
    }
    // if (isBotSender(payload)) {
    //   core.info("来自 bot 的评论事件，忽略");
    //   return;
    // }
    // const number = payload.discussion.number;
    // await consumeComments(number, (comment) => {
    //   return false;
    // });
  }
}

run().catch((err) => {
  core.setFailed(err.message);
});
