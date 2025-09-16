#!/bin/bash

# SWITCH Git Workflow Script
# Provides common Git operations for the project

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[SWITCH]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if we're in a Git repository
check_git_repo() {
    if ! git rev-parse --git-dir > /dev/null 2>&1; then
        print_error "Not in a Git repository!"
        exit 1
    fi
}

# Function to check if we're on the correct branch
check_branch() {
    local expected_branch=$1
    local current_branch=$(git branch --show-current)
    
    if [ "$current_branch" != "$expected_branch" ]; then
        print_warning "Expected to be on '$expected_branch' but currently on '$current_branch'"
        read -p "Continue anyway? (y/N): " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            exit 1
        fi
    fi
}

# Function to create a feature branch
create_feature_branch() {
    local sprint=$1
    local component=$2
    
    if [ -z "$sprint" ] || [ -z "$component" ]; then
        print_error "Usage: create_feature_branch <sprint> <component>"
        print_error "Example: create_feature_branch 1 gamemanager"
        exit 1
    fi
    
    local branch_name="feature/sprint-${sprint}-${component}"
    
    print_status "Creating feature branch: $branch_name"
    
    # Check if branch already exists
    if git show-ref --verify --quiet refs/heads/$branch_name; then
        print_warning "Branch '$branch_name' already exists!"
        read -p "Switch to existing branch? (y/N): " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            git checkout $branch_name
        else
            exit 1
        fi
    else
        git checkout -b $branch_name
        print_success "Created and switched to branch: $branch_name"
    fi
}

# Function to commit changes with conventional format
commit_changes() {
    local sprint=$1
    local type=$2
    local description=$3
    local issue_number=$4
    
    if [ -z "$sprint" ] || [ -z "$type" ] || [ -z "$description" ]; then
        print_error "Usage: commit_changes <sprint> <type> <description> [issue_number]"
        print_error "Example: commit_changes 1 feat 'Implement GameManager singleton' 123"
        exit 1
    fi
    
    local commit_message="[Sprint $sprint] $type: $description"
    
    if [ -n "$issue_number" ]; then
        commit_message="$commit_message\n\nCloses #$issue_number"
    fi
    
    print_status "Committing changes..."
    print_status "Message: $commit_message"
    
    # Check if there are changes to commit
    if git diff --staged --quiet; then
        print_warning "No staged changes to commit!"
        exit 1
    fi
    
    git commit -m "$commit_message"
    print_success "Changes committed successfully!"
}

# Function to push branch and create PR
push_and_pr() {
    local branch_name=$(git branch --show-current)
    
    print_status "Pushing branch: $branch_name"
    git push origin $branch_name
    
    print_success "Branch pushed successfully!"
    
    # Check if GitHub CLI is available
    if command -v gh &> /dev/null; then
        print_status "Creating pull request..."
        gh pr create --title "$branch_name" --body "See PR template for details"
        print_success "Pull request created!"
    else
        print_warning "GitHub CLI not found. Please create PR manually at:"
        print_warning "https://github.com/uriahwhitt/SWITCH/compare/$branch_name"
    fi
}

# Function to run pre-commit checks
pre_commit_checks() {
    print_status "Running pre-commit checks..."
    
    # Check if we're in the right directory
    if [ ! -f "SWITCH_PRD_Final.md" ]; then
        print_error "Not in SWITCH project root directory!"
        exit 1
    fi
    
    # Run tests
    if [ -f "run-tests.sh" ]; then
        print_status "Running tests..."
        ./run-tests.sh
        if [ $? -ne 0 ]; then
            print_error "Tests failed!"
            exit 1
        fi
        print_success "Tests passed!"
    fi
    
    # Update planning context
    if [ -f "planning-context/update-planning-context.sh" ]; then
        print_status "Updating planning context..."
        ./planning-context/update-planning-context.sh
        print_success "Planning context updated!"
    fi
    
    # Check for uncommitted changes
    if ! git diff --quiet; then
        print_warning "Uncommitted changes detected!"
        git status --short
        read -p "Stage and commit these changes? (y/N): " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            git add .
            read -p "Enter commit message: " commit_msg
            git commit -m "$commit_msg"
        fi
    fi
    
    print_success "Pre-commit checks completed!"
}

# Function to show current status
show_status() {
    print_status "Current Git status:"
    echo
    
    # Current branch
    local current_branch=$(git branch --show-current)
    print_status "Current branch: $current_branch"
    
    # Status
    git status --short
    
    # Recent commits
    echo
    print_status "Recent commits:"
    git log --oneline -5
    
    # Sprint status
    if [ -f "planning-context/SPRINT_STATUS.md" ]; then
        echo
        print_status "Current sprint status:"
        grep -A 5 "Current Sprint:" planning-context/SPRINT_STATUS.md || true
    fi
}

# Main script logic
case "$1" in
    "create-branch")
        create_feature_branch "$2" "$3"
        ;;
    "commit")
        commit_changes "$2" "$3" "$4" "$5"
        ;;
    "push")
        push_and_pr
        ;;
    "pre-commit")
        pre_commit_checks
        ;;
    "status")
        show_status
        ;;
    "help"|"--help"|"-h"|"")
        echo "SWITCH Git Workflow Script"
        echo
        echo "Usage: $0 <command> [arguments]"
        echo
        echo "Commands:"
        echo "  create-branch <sprint> <component>  Create a new feature branch"
        echo "  commit <sprint> <type> <description> [issue]  Commit changes with conventional format"
        echo "  push                                Push current branch and create PR"
        echo "  pre-commit                          Run pre-commit checks"
        echo "  status                              Show current Git status"
        echo "  help                                Show this help message"
        echo
        echo "Examples:"
        echo "  $0 create-branch 1 gamemanager"
        echo "  $0 commit 1 feat 'Implement GameManager singleton'"
        echo "  $0 push"
        echo "  $0 pre-commit"
        echo "  $0 status"
        ;;
    *)
        print_error "Unknown command: $1"
        echo "Use '$0 help' for usage information"
        exit 1
        ;;
esac
