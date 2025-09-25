#!/bin/bash

echo "Updating SWITCH Planning Context..."

# Update timestamp
echo "Last Updated: $(date)" > CONTEXT_SUMMARY.md
echo "" >> CONTEXT_SUMMARY.md

# Copy essential files
cp ../SWITCH_PRD_Final.md ./Product_Requirement_Document.md
echo "✓ Product_Requirement_Document.md copied" >> CONTEXT_SUMMARY.md

# Create placeholder files if they don't exist
if [ ! -f "./SPRINT_PLAN.md" ]; then
    echo "# SWITCH Sprint Plan" > SPRINT_PLAN.md
    echo "## Sprint 1: Foundation (Weeks 1-2)" >> SPRINT_PLAN.md
    echo "- Unity project architecture" >> SPRINT_PLAN.md
    echo "- Grid and matching system" >> SPRINT_PLAN.md
    echo "- Directional gravity core" >> SPRINT_PLAN.md
    echo "- Basic tile graphics" >> SPRINT_PLAN.md
    echo "✓ SPRINT_PLAN.md created" >> CONTEXT_SUMMARY.md
fi

if [ ! -f "./SPRINT_STATUS.md" ]; then
    echo "# SWITCH Sprint Status" > SPRINT_STATUS.md
    echo "## Current Sprint: Sprint 0 - Project Setup" >> SPRINT_STATUS.md
    echo "**Week**: 0 of 0" >> SPRINT_STATUS.md
    echo "**Day**: 1 of 1" >> SPRINT_STATUS.md
    echo "**Overall Progress**: 0%" >> SPRINT_STATUS.md
    echo "✓ SPRINT_STATUS.md created" >> CONTEXT_SUMMARY.md
fi

if [ ! -f "./ARCHITECTURE.md" ]; then
    echo "# SWITCH Architecture" > ARCHITECTURE.md
    echo "## Unity Architecture Overview" >> ARCHITECTURE.md
    echo "## Core Patterns" >> ARCHITECTURE.md
    echo "## Performance Requirements" >> ARCHITECTURE.md
    echo "✓ ARCHITECTURE.md created" >> CONTEXT_SUMMARY.md
fi

if [ ! -f "./DECISIONS.md" ]; then
    echo "# SWITCH Technical Decisions" > DECISIONS.md
    echo "## Decision Log" >> DECISIONS.md
    echo "## Architecture Decisions" >> DECISIONS.md
    echo "## Performance Decisions" >> DECISIONS.md
    echo "✓ DECISIONS.md created" >> CONTEXT_SUMMARY.md
fi

cp ../SWITCH_Cursor_rules.md ./Cursor_rules.md
echo "✓ Cursor_rules.md copied" >> CONTEXT_SUMMARY.md

# Copy current implementation files if they exist
if [ -f "../SWITCH/Assets/_Project/Scripts/Core/GameManager.cs" ]; then
    cp ../SWITCH/Assets/_Project/Scripts/Core/GameManager.cs ./
    echo "✓ GameManager.cs copied" >> CONTEXT_SUMMARY.md
fi

if [ -f "../SWITCH/Assets/_Project/Scripts/Mechanics/DirectionalGravity.cs" ]; then
    cp ../SWITCH/Assets/_Project/Scripts/Mechanics/DirectionalGravity.cs ./
    echo "✓ DirectionalGravity.cs copied" >> CONTEXT_SUMMARY.md
fi

if [ -f "../SWITCH/Assets/_Project/Scripts/Mechanics/QueueSystem.cs" ]; then
    cp ../SWITCH/Assets/_Project/Scripts/Mechanics/QueueSystem.cs ./
    echo "✓ QueueSystem.cs copied" >> CONTEXT_SUMMARY.md
fi

echo "" >> CONTEXT_SUMMARY.md
echo "Essential Files Status:" >> CONTEXT_SUMMARY.md
echo "✓ Product_Requirement_Document.md" >> CONTEXT_SUMMARY.md
echo "✓ SPRINT_PLAN.md" >> CONTEXT_SUMMARY.md
echo "✓ SPRINT_STATUS.md" >> CONTEXT_SUMMARY.md
echo "✓ ARCHITECTURE.md" >> CONTEXT_SUMMARY.md
echo "✓ DECISIONS.md" >> CONTEXT_SUMMARY.md
echo "✓ Cursor_rules.md" >> CONTEXT_SUMMARY.md

echo "Planning context updated successfully!"
