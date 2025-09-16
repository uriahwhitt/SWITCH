# SWITCH Planning Context

This directory contains all essential files for AI planning and implementation coordination.

## Essential Files (Always Include)
- `Product_Requirement_Document.md` - Complete product vision and requirements
- `SPRINT_PLAN.md` - Full roadmap and sprint breakdown
- `SPRINT_STATUS.md` - Current sprint/week/day progress
- `ARCHITECTURE.md` - Unity architecture and patterns
- `DECISIONS.md` - Technical decisions log
- `Cursor_rules.md` - Division of responsibilities

## Conditional Files (Include When Relevant)
- `GameManager.cs` - Core game controller (when exists)
- `DirectionalGravity.cs` - Core mechanic implementation
- `QueueSystem.cs` - Queue implementation
- `PowerUpSystem.cs` - Power-up implementation
- `API_Specifications.md` - Backend API docs

## Usage
Run `./update-planning-context.sh` to sync all current files into this directory for AI context.
