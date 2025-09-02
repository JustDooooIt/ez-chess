import * as github from "@actions/github";
import * as core from "@actions/core";

const { context } = github;
const { repo: repoInfo } = context;
const owner = repoInfo.owner;
const repo = repoInfo.repo;
const token = process.env.GITHUB_TOKEN;
const octokit = github.getOctokit(token!);

const QUEUE_ISSUE_NUMBER = 94;
const discussionNumber = process.env.DISCUSSION_NUMBER;
const TASK_PREFIX = `TASK::${discussionNumber}::`;

