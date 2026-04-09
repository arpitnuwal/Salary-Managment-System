const fs = require("fs");
const path = require("path");
const { execSync } = require("child_process");
const readline = require("readline");

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

// Random commit messages
const messages = [
    "auto update",
    "minor fix",
    "code improved",
    "bug fixed",
    "UI updated",
    "README changed",
    "performance enhanced",
    "small tweaks",
    "feature refined",
    "layout adjusted",
    "code cleanup",
    "logic optimized",
    "style improved",
    "new changes added",
    "function updated",
    "issue resolved",
    "docs updated",
    "comments added",
    "refactor complete",
    "responsive fix"
];

function getRandomMessage() {
    const randomText = messages[Math.floor(Math.random() * messages.length)];
    const randomNumber = Math.floor(Math.random() * 900) + 100;
    const currentTime = new Date().toLocaleTimeString();

    return `${randomText}-${randomNumber}-${currentTime}`;
}

function askQuestion(question, defaultValue) {
    return new Promise((resolve) => {
        rl.question(`${question} (default ${defaultValue}): `, (answer) => {
            resolve(answer.trim() || defaultValue);
        });
    });
}

function makeCommit(repoPath, filename) {
    const filePath = path.join(repoPath, filename);

    // Random file update
    fs.appendFileSync(
        filePath,
        `Update: ${new Date().toISOString()} - ${Math.random()}\n`
    );

    const commitMessage = getRandomMessage();

    console.log(`📝 Commit Message: ${commitMessage}`);

    // Git add & commit
    execSync(`git add .`, { cwd: repoPath, stdio: "inherit" });
    execSync(`git commit -m "${commitMessage}"`, {
        cwd: repoPath,
        stdio: "inherit"
    });
}

async function main() {
    console.log("=".repeat(60));
    console.log("🌱 GitHub Contribution Generator 🌱");
    console.log("=".repeat(60));

    const numCommits = parseInt(
        await askQuestion("How many commits do you want to make", "20")
    );
    const repoPath = await askQuestion(
        "Enter your local git repo path",
        "."
    );
    const filename = await askQuestion(
        "Enter file name",
        "README.md"
    );

    console.log(`\nMaking ${numCommits} random commits...\n`);

    for (let i = 0; i < numCommits; i++) {
        console.log(`[${i + 1}/${numCommits}] Committing...`);
        makeCommit(repoPath, filename);
    }

    console.log("\n🚀 Pushing commits...");
    execSync("git push", { cwd: repoPath, stdio: "inherit" });

    console.log("✅ Done! GitHub contribution green ho jayega 😄");

    rl.close();
}

main();