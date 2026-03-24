const fs = require("fs");
const path = require("path");
const { execSync } = require("child_process");
const readline = require("readline");

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

// Better commit messages (realistic)
const messages = [
    "feat: added new feature",
    "fix: resolved bug",
    "docs: updated documentation",
    "refactor: improved code structure",
    "style: formatting changes",
    "perf: performance improved",
    "test: added test cases",
    "chore: minor maintenance",
];

// Task content generator
const tasks = [
    "Improve UI layout",
    "Fix login issue",
    "Optimize API call",
    "Refactor function logic",
    "Add validation",
    "Update README",
    "Enhance responsiveness"
];

// Utility: random item
function randomItem(arr) {
    return arr[Math.floor(Math.random() * arr.length)];
}

// Utility: delay
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

// Generate random past date
function getRandomDate(daysBack = 30) {
    const date = new Date();
    date.setDate(date.getDate() - Math.floor(Math.random() * daysBack));
    return date.toISOString();
}

// Ask input
function ask(question, def) {
    return new Promise(resolve => {
        rl.question(`${question} (default ${def}): `, ans => {
            resolve(ans.trim() || def);
        });
    });
}

// Make commit
function makeCommit(repoPath, useEmptyCommit) {
    const commitMessage = randomItem(messages);
    const randomDate = getRandomDate();

    console.log(`📝 ${commitMessage} | 📅 ${randomDate}`);

    if (!useEmptyCommit) {
        const file = path.join(repoPath, "activity.txt");

        const content = `✔ ${randomItem(tasks)} - ${new Date().toISOString()}\n`;
        fs.appendFileSync(file, content);

        execSync("git add .", { cwd: repoPath, stdio: "inherit" });
    }

    execSync(
        `git commit ${useEmptyCommit ? "--allow-empty" : ""} --date="${randomDate}" -m "${commitMessage}"`,
        { cwd: repoPath, stdio: "inherit" }
    );
}

// Main function
async function main() {
    console.log("=".repeat(50));
    console.log("🚀 Smart GitHub Contribution Script");
    console.log("=".repeat(50));

    const totalCommits = parseInt(await ask("Total commits", "15"));
    const repoPath = await ask("Repo path", ".");
    const useEmpty = (await ask("Use empty commits? (yes/no)", "no")) === "yes";

    console.log("\n⚡ Starting commits...\n");

    for (let i = 0; i < totalCommits; i++) {
        console.log(`➡️ Commit ${i + 1}/${totalCommits}`);
        makeCommit(repoPath, useEmpty);

        // random delay (1–3 sec)
        await sleep(Math.floor(Math.random() * 2000) + 1000);
    }

    console.log("\n🚀 Pushing to GitHub...");
    execSync("git push", { cwd: repoPath, stdio: "inherit" });

    console.log("✅ Done! Your contributions updated 😎");

    rl.close();
}

main();