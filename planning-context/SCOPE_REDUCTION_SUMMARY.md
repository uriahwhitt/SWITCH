# SWITCH Scope Reduction Summary

## Overview
This document summarizes the comprehensive scope reduction from a complex multiplayer game to a simplified single-player MVP with basic social sharing features.

## Key Changes Made

### 1. Architecture Simplification
- **Removed**: P2P validation systems, PeerValidator, ScoreVerifier, LeaderboardSync
- **Added**: SimpleLeaderboardService, FriendCodeSystem, ShareState
- **Simplified**: NetworkManager to handle only basic HTTP calls and social features

### 2. Timeline Reduction
- **From**: 8 weeks (4 sprints)
- **To**: 6 weeks (3 sprints)
- **Focus**: Core gameplay mechanics + simple social features

### 3. Monetization Simplification
- **Primary Revenue**: Watch ad to continue (core revenue stream)
- **Secondary Revenue**: Premium upgrade ($4.99 removes ads)
- **Removed**: Power-up packs, cosmetics, complex monetization

### 4. Social Features Simplification
- **Added**: Friend codes (6-character alphanumeric)
- **Added**: Social media sharing (Twitter, Facebook, Instagram)
- **Added**: Simple leaderboards (global top 100, friends only)
- **Removed**: Ghost challenges, replay system, challenge notifications
- **Removed**: Complex friend system, in-game friend search
- **Removed**: Spectator features, clubs/guilds

### 5. Backend Simplification
- **From**: Complex P2P validation with peer networks
- **To**: Simple Firebase structure:
  ```
  /players/{playerId}
    - friendCode: string
    - displayName: string
    - highScore: number
    - friends: string[] (friend codes)

  /leaderboards/global/{scoreId}
    - playerId: string
    - score: number
    - timestamp: number

  /leaderboards/friends/{playerId}
    - Shows filtered view of global based on friend list
  ```

## Documents Updated

### Architecture Documents
- ✅ `docs/architecture/SWITCH_system_architecture.md`
  - Removed P2P validation flows
  - Added SimpleLeaderboardService classes
  - Updated NetworkManager to simplified version

### Product Requirements
- ✅ `planning-context/SWITCH_PRD_Final.md`
  - Updated leaderboard system to simple version
  - Added social sharing features section
  - Updated monetization to simplified model
  - Changed timeline to 6 weeks
  - Updated success metrics for single-player focus

### UML Diagrams
- ✅ `docs/diagrams/uml/03_state_diagrams/01_game_state_machine.mmd`
  - Removed NetworkValidationState
  - Added ShareState for social media sharing
  - Updated GameOverState to include sharing

- ✅ `docs/diagrams/uml/02_sequence_diagrams/04_simple_score_submission.mmd`
  - Created new sequence diagram for simplified score submission
  - Added social sharing flows
  - Added friend code operations

- ✅ `docs/diagrams/uml/06_use_case_diagrams/01_use_cases_mvp.mmd`
  - Created new MVP use case diagram
  - Focused on single-player features
  - Added simple social features
  - Excluded complex multiplayer features

- ✅ `docs/diagrams/uml/01_class_diagrams/01_core_game_architecture.mmd`
  - Updated NetworkManager class
  - Added SimpleLeaderboardService, FriendCodeSystem, LeaderboardEntry
  - Added ShareState class
  - Updated relationships

### Planning Documents
- ✅ `planning-context/SPRINT_PLAN.md`
  - Updated to 6-week timeline
  - Reorganized sprints to focus on core mechanics first
  - Added social features to final sprint

- ✅ `planning-context/implementation.md`
  - Added simplified social features section
  - Updated forbidden practices to exclude P2P
  - Added MVP scope context

## Core Game Mechanics (Unchanged)
- ✅ Directional gravity system
- ✅ 10-dot queue system
- ✅ Anti-frustration algorithm
- ✅ Shape-based accessibility
- ✅ Progressive tutorial hints
- ✅ Power-up system (5 basic types)
- ✅ Match detection and cascades

## New Simple Features Added
- ✅ ShareToSocial() method in GameOverState
- ✅ CopyFriendCode() button in settings
- ✅ PasteFriendCode() to add friends
- ✅ ViewFriendsLeaderboard() filtered view
- ✅ Social media sharing integration
- ✅ Simple server-side score validation

## Benefits of Scope Reduction
1. **Faster Development**: 6 weeks vs 8 weeks
2. **Lower Complexity**: No P2P networking complexity
3. **Reduced Backend Costs**: Simple Firebase vs complex peer validation
4. **Easier Testing**: Single-player focus
5. **Faster Time to Market**: Can ship and iterate
6. **Viral Growth Potential**: Social sharing for external growth
7. **Proven Core Loop**: Focus on gameplay that works

## Success Metrics (Updated)
- **Single-Player Retention**: D1 >40%, D7 >20%, D30 >10%
- **Social Sharing**: >10% of players share scores
- **Downloads**: 5,000 in first month (reduced from 10,000)
- **Focus**: 90% single-player experience with viral growth potential

## Next Steps
1. Begin Sprint 1: Core Mechanics (Weeks 1-2)
2. Implement directional gravity system
3. Build 10-dot queue system
4. Add basic power-ups
5. Implement simple social features
6. Launch MVP and gather feedback
7. Iterate based on player response

This scope reduction maintains the core innovative gameplay while significantly reducing development complexity and time to market.
