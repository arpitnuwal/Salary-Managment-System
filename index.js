const fs = require("fs");
const path = require("path");
const { execSync } = require("child_process");
const readline = require("readline");

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

// Realistic commit messages
const messages = [
    "feat: add new feature",
    "fix: resolve issue",
    "docs: update docs",
    "refactor: improve code",
    "style: formatting",
    "perf: optimize performance",
    "test: add tests",
    "chore: cleanup"
];

// Realistic tasks
const tasks = [
    "Improve UI",
    "Fix auth bug",
    "Optimize API",
    "Refactor logic",
    "Add validation",
    "Update README",
    "Enhance UX"
];

// Random item
function randomItem(arr) {
    return arr[Math.floor(Math.random() * arr.length)];
}

// Delay
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

// 🔥 SMART DATE (spread across days + random time)
function getSmartDate(index, totalDays = 120) {
    const date = new Date();

    // Spread commits across past days
    const dayOffset = Math.floor((index / totalDays) * totalDays);
    date.setDate(date.getDate() - dayOffset);

    // Random time in day
    date.setHours(
        Math.floor(Math.random() * 24),
        Math.floor(Math.random() * 60),
        Math.floor(Math.random() * 60)
    );

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
function makeCommit(repoPath, index, useEmptyCommit) {
    const message = randomItem(messages);
    const commitDate = getSmartDate(index);

    console.log(`📝 ${message} | 📅 ${commitDate}`);

    if (!useEmptyCommit) {
        const file = path.join(repoPath, "activity.txt");

        const content = `✔ ${randomItem(tasks)} - ${new Date().toISOString()}\n`;
        fs.appendFileSync(file, content);

        execSync("git add .", { cwd: repoPath, stdio: "inherit" });
    }

    execSync(
        `git commit ${useEmptyCommit ? "--allow-empty" : ""} --date="${commitDate}" -m "${message}"`,
        { cwd: repoPath, stdio: "inherit" }
    );
}

// Main
async function main() {
    console.log("=".repeat(50));
    console.log("🚀 Smart Contribution Generator v2");
    console.log("=".repeat(50));

    const totalCommits = parseInt(await ask("Total commits", "100"));
    const repoPath = await ask("Repo path", ".");
    const useEmpty = (await ask("Empty commits? (yes/no)", "no")) === "yes";

    console.log("\n⚡ Generating commits...\n");

    for (let i = 0; i < totalCommits; i++) {
        console.log(`➡️ Commit ${i + 1}/${totalCommits}`);

        makeCommit(repoPath, i, useEmpty);

        // 🔥 random delay (2–6 sec)
        await sleep(Math.floor(Math.random() * 4000) + 2000);
    }

    console.log("\n🚀 Pushing...");
    execSync("git push", { cwd: repoPath, stdio: "inherit" });

    console.log("✅ Done! Check GitHub after 10–20 min 😎");

    rl.close();
}

main();