import * as github from "@actions/github";
import * as core from "@actions/core";


const { context } = github;
const { repo: repoInfo, payload, issue } = context;
const owner = repoInfo.owner;
const repo = repoInfo.repo;
const token = process.env.GITHUB_TOKEN;
const octokit = github.getOctokit(token);

const QUEUE_ISSUE_NUMBER = 93;process.env
const discussionNumber = process.env.DISCUSSION_NUMBER;
const TASK_PREFIX = `TASK::${discussionNumber}::`;

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
  let observers = new Set(jsonObject.observers);
  observers.add(payload.sender.login);
  jsonObject.observers = Array.from(observers);
  let json = JSON.stringify(jsonObject);
  await updateDiscussion(payload.discussion.node_id, json);
}

async function OnSelectFaction(faction) {
  let jsonObject = JSON.parse(payload.discussion.body);
  let observers = new Set(jsonObject.observers);
  observers.delete(payload.sender.login);
  jsonObject.observers = Array.from(observers);
  jsonObject.seats[faction] = payload.sender.login;
  let json = JSON.stringify(jsonObject);
  await updateDiscussion(payload.discussion.node_id, json);
}

async function run() {
  const comments = await octokit.paginate(octokit.rest.issues.listComments, {
    owner,
    repo,
    issue_number: QUEUE_ISSUE_NUMBER,
    per_page: 100,
  });

  let taskToProcess = null;
  for (const comment of comments) {
    if (comment.body.startsWith(TASK_PREFIX) && !comment.body.includes("~~")) {
      taskToProcess = comment;
      break; 
    }
  }

  let commentUrl = taskToProcess.body.split("::").pop();
  
  const commentIdFragment = commentUrl.split('#').pop();
  
  const commentNumericId = commentIdFragment.split('-').pop();

  core.info(payload.comment.id)


  // if (taskToProcess.body == "/enter") {
  //   await OnEnterRoom();
  // } else if (taskToProcess.body?.startsWith("/choose/faction")) {
  //   let faction = taskToProcess?.body?.split("/")?.pop();
  //   await OnSelectFaction(faction);
  // }

  const updatedBody = `~~${taskToProcess.body.trim()}~~ --- Processed in run ${context.runId}`;
  await octokit.rest.issues.updateComment({
    owner, repo, comment_id: taskToProcess.id, body: updatedBody,
  });
}

run().catch((err) => {
  core.setFailed(err.message);
});
