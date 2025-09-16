# SWITCH - Strategic Wisdom In Tile Clearing Heuristics - 1.3 Product Requirements Document
**Whitt's End, LLC.**  
**Version: 1.3 - COMPLETE PRODUCT VISION**  
**Date: September 16 2025**  
**Status: APPROVED FOR DEVELOPMENT**

## 1. Executive Summary

### 1.1 Product Vision
SWITCH revolutionizes the endless puzzle genre by combining match-3 mechanics with Tetris-style strategic planning through player-controlled directional gravity, creating the first truly strategic endless match-3 experience.

### 1.2 Mission Statement
Create an endlessly replayable puzzle game where every decision matters, combining the satisfaction of match-3 with the strategic depth of block puzzlers, rewarding thoughtful play over random luck.

### 1.3 Core Value Proposition
- **For Players**: Skill-based endless gameplay where strategy beats luck
- **For Competitive Players**: True high-score competition based on mastery
- **For Puzzle Fans**: Fresh mechanics that feel both familiar and revolutionary

## 2. Problem Statement

### 2.1 Core Problems

**Problem 1: Artificial Difficulty in Match-3**
- Traditional match-3 games use randomness to force losses
- "The game decided you should lose" frustration
- Pay-to-win mechanics disguised as difficulty

**Problem 2: Lack of Strategic Planning**
- No ability to plan ahead in traditional match-3
- Purely reactive gameplay
- Limited player agency over outcomes

**Problem 3: Endless Mode Limitations**
- Most match-3 games lack satisfying endless modes
- Tetris-style games lack the satisfaction of matching
- No games successfully merge both genres

### 2.2 Evidence & Research Requirements
[TO BE COMPLETED PRE-DEVELOPMENT]
- Block Blast player retention metrics
- Tetris endless mode engagement statistics  
- Match-3 player churn at "impossible" levels
- Skill vs. luck preference studies in puzzle games
- High-score leaderboard engagement metrics

### 2.3 Success Validation Metrics
- Skill Gap Validation: Top 10% players score 5x+ higher than average
- Strategic Depth: Average move consideration time >3 seconds
- Fairness Perception: <5% of players report "unfair" losses
- Endless Engagement: Average session >15 minutes

## 3. Target Market

### 3.1 Primary Audience
**Strategic Puzzle Players** (40% of target)
- Play: Tetris 99, Block Blast, 2048
- Want: Skill-based progression
- Age: 25-45
- Mindset: Competitive, analytical

**Frustrated Match-3 Players** (35% of target)
- Play: Candy Crush but hate pay-to-win
- Want: Fair, endless gameplay
- Age: 30-50
- Mindset: Casual but skilled

**Mobile Puzzle Enthusiasts** (25% of target)
- Play: Multiple puzzle games daily
- Want: Fresh mechanics
- Age: 18-35
- Mindset: Early adopters, variety seekers

### 3.2 Geographic Strategy
**Global Launch Strategy** (Digital Distribution)
- Primary: English-speaking markets
- Secondary: Europe, Asia (no localization MVP)
- Platform: iOS & Android simultaneous

### 3.3 Competitive Landscape
| Game | Strengths | Weaknesses | SWITCH Advantage |
|------|-----------|------------|-------------------|
| Block Blast | Strategic depth | No matching satisfaction | Match-3 satisfaction + strategy |
| Tetris | Skill-based | Dated mechanics | Modern mechanics + planning |
| Candy Crush | Massive audience | Pay-to-win frustration | Fair, skill-based progression |
| 2048 | Pure skill | Limited mechanics | Richer gameplay variety |

## 4. Product Features

### 4.1 Core Innovation: Directional Gravity System
**Revolutionary Mechanic**
- Player controls where new blocks enter through swipe direction
- LEFT swipe → blocks flow from RIGHT
- RIGHT swipe → blocks flow from LEFT  
- UP swipe → blocks flow from BOTTOM
- DOWN swipe → blocks flow from TOP

**Strategic Impact**
- Every move affects future board state
- Multiple solutions for any situation
- Skill directly translates to higher scores

### 4.2 Block Queue System
**10-Dot Vertical Queue Mechanics**

**Visual Layout:**
```
Left Side of Screen:
┌─┐
│●│ <- Top (10th position) - Newest
│●│
│○│
│●│
│●│
│○│
│●│
│●│
│○│
│●│ <- Bottom (1st position) - Next to play
└─┘
↓ (Drops to board)
```

**Queue Flow Sequence:**
1. Player makes match selection (swipe direction)
2. Bottom 3+ dots "fall" from queue to board edge
3. Tiles enter from directional edge
4. Board evaluates for matches
5. Matches clear with animation
6. New dots calculated based on board state
7. New dots drop into top of queue
8. Check for cascade matches
9. If cascades found, repeat from step 5
10. Turn complete, await player input

**Queue Animation Details:**
- Drop Animation: 0.3s smooth fall from queue to board
- Refill Animation: 0.2s new dots dropping from above
- Cascade Pause: 0.5s between cascade evaluations
- Visual Feedback: Dots glow when about to deploy

### 4.3 Center-Flow Pressure System
**Spatial Strategy Layer**
- Board zones: Edge (1x), Transition (1.5x), Center (2x)
- Natural pressure toward center over time
- Risk/reward for center positioning
- Pressure release through strategic matches

### 4.4 Endless Survival Mode (Primary Game Mode)
**Core Gameplay Loop**
1. View block queue
2. Analyze board state  
3. Choose move direction
4. Execute match
5. New blocks flow from chosen direction
6. Score multiplies with survival time
7. Speed gradually increases
8. Game ends when no moves available

**Difficulty Progression**
- Speed increase every 60 seconds
- Pressure intensity grows over time
- Special blocks appear at milestones
- No artificial difficulty walls

### 4.5 Daily Challenges & Events
**Daily Challenge Types**
- Fixed Seed Challenge: Everyone gets same block sequence
- Speed Run: Hit target score in time limit
- Precision Mode: Limited moves, maximum score
- Survival Challenge: Reach time milestones

**Weekly Events**
- Tournament brackets
- Community goals
- Special rule modifiers
- Seasonal themes

### 4.6 Progression & Engagement Systems
**Player Level System**
- XP earned through gameplay
- Levels unlock new features
- No gameplay advantages
- Profile progression tracking

**Achievement System**
- Skill-based achievements
- Hidden achievements for discovery
- Statistics tracking
- Personal best tracking

### 4.7 Simple Leaderboard System
**Leaderboard Types**
- Global all-time high scores (top 100)
- Friends leaderboard (via friend codes)
- Daily personal best tracking

**Simple Anti-Cheat Measures**
- Server-side basic validation
- Statistical anomaly detection
- No complex replay system needed

**Friend Code System**
- 6-character alphanumeric codes
- Players share codes outside the app
- Friend leaderboard shows scores of added friends
- No in-game messaging or challenges

### 4.8 Social Sharing Features
**Share to Social Media**
- Share button posts score screenshot to social media
- Pre-formatted messages with game branding
- Twitter, Facebook, Instagram integration
- Viral growth through external sharing

**Social Features**
- CopyFriendCode() button in settings
- PasteFriendCode() to add friends
- ViewFriendsLeaderboard() filtered view
- ShareToSocial() method in GameOverState

### 4.9 Intelligent Tile Distribution System
**Anti-Frustration Algorithm**

**Distribution Rules:**
```yaml
Minimum Viability:
  - Always ensure at least 1 valid match possible
  - Monitor "match drought" counter
  - Intervene if no matches made in 5 moves

Pattern Prevention:
  - No more than 4 consecutive identical colors
  - Avoid creating isolated single tiles
  - Prevent "checkboard" anti-patterns
  
Adaptive Difficulty:
  - Track player skill level (average score)
  - Adjust distribution based on performance
  - More variety for skilled players
  - More matches for struggling players
```

**Board State Analysis:**
```yaml
Analysis Metrics:
  - Match Potential Score: Available matches on board
  - Color Distribution: Balance across all colors
  - Cluster Quality: Groups of same-color tiles
  - Edge Pressure: Tiles building up on edges
  - Center Density: Center zone saturation

Response Adjustments:
  - Low match potential → Increase matching colors by 40%
  - Poor distribution → Force variety in next 6 blocks
  - No clusters → Increase same-color probability
  - Edge pressure → Provide clearing opportunities
  - Center congestion → Offer strategic colors
```

### 4.10 Power-Up System
**Progressive Power-Up Mechanics**

**Power-Up Categories:**
```yaml
Queue Manipulation:
  - Queue Shuffle: Randomize next 5 dots
  - Queue Delete: Remove 1 specific dot
  - Queue Delay: Push 1 dot to position 10
  - Queue Peek: Reveal 5 additional future dots
  - Queue Swap: Exchange 2 dots positions

Board Powers:
  - Color Bomb: Clear all of one color
  - Row Clear: Eliminate entire row
  - Column Clear: Eliminate entire column  
  - Rainbow Tile: Matches with any color
  - Gravity Reverse: One-time opposite flow

Emergency Powers:
  - Undo Move: Revert last action
  - Board Shuffle: Randomize all tiles
  - Time Freeze: Pause timer for 30 seconds
  - Match Hint: Highlight best move
  - Safety Net: Prevent next game over
```

**Power-Up Earning System:**
```yaml
End-of-Run Rewards:
  Score Thresholds:
    - 10,000 points: 1 random basic power
    - 25,000 points: 1 chosen basic power
    - 50,000 points: 1 random advanced power
    - 100,000 points: 1 chosen advanced power

  Achievement Unlocks:
    - First 5-match: Queue Peek
    - Survive 5 minutes: Time Freeze
    - 10 cascades in one run: Color Bomb
    - Center zone master: Gravity Reverse
    - No powers used + 25k score: Queue Shuffle

Power-Up Inventory:
  - Maximum capacity: 20 total powers
  - Stack limit: 5 of same type
  - Persistent between runs
  - No use limit per run
  - Powers affect scoring
```

### 4.11 Accessibility System
**Shape-Based Color System**

**Primary Shape Mapping:**
```
Base Colors & Shapes:
■ = Red (Square)
● = Blue (Circle)
▲ = Yellow (Triangle)
♦ = Orange (Diamond)
★ = Green (Star)
⬟ = Violet (Hexagon)
```

**Shape Modifiers for Shades:**
```
Light Shade: Hollow shape
Dark Shade: Filled with pattern
Special Variants: Internal symbols for power tiles
```

**Display Modes:**
- Standard Mode: Colors only (default)
- Accessible Mode: Colors + Shapes
- Shape-Only Mode: Black & white with shapes only

### 4.12 Tutorial System
**Progressive Hint System**

**Tutorial Philosophy:**
- No forced tutorial level
- Learn through playing
- Context-sensitive hints
- Can disable anytime

**Hint Triggers:**
- First-time events (swipe, queue, power-ups)
- Struggle detection (no match in 10 seconds)
- Advanced tips unlocked by progress
- Visual and text hints available

## 5. Technical Requirements

### 5.1 Technology Stack
**Selected Framework: Unity 2022.3 LTS**
- Mature 2D game development tools
- Excellent mobile performance optimization
- Strong asset pipeline for puzzle games
- Extensive documentation and community
- Complements existing React skills (different domain)

### 5.2 Performance Requirements
```yaml
Core Metrics:
  Frame rate: 60 FPS target, 30 FPS minimum
  Input latency: <100ms
  Load time: <5 seconds
  Battery impact: <10% per hour
  Memory usage: <200MB

Device Support:
  iOS: 13.0+ (5 years back)
  Android: API 26+ (Android 8.0+)
  Screen sizes: 4.7" to 12.9"
  Orientation: Portrait primary
```

### 5.3 Backend Architecture
**Essential Backend Services**
- Leaderboard management
- Daily challenge distribution
- Anti-cheat validation
- Analytics collection
- Cloud save sync

**Backend Options**
- Firebase (fast MVP, Unity integration)
- AWS Amplify (scalable, more complex)
- Custom Node.js (maximum control)

### 5.4 Screen Layout Architecture
```
Portrait Mode (Primary):
┌────────────────────────────┐
│      Score | Timer         │ 5%
├──┬─────────────────────────┤
│Q │                         │
│u │                         │
│e │      8x8 Game Grid      │ 70%
│u │                         │
│e │                         │
│10│                         │
├──┴────┬────────────────────┤
│Powers │ Next Move Preview  │ 15%
│[][][]│  Direction Arrow    │
├───────┴────────────────────┤
│  Pause │ Settings │ Menu   │ 5%
└────────────────────────────┘
```

## 6. Monetization Strategy

### 6.1 Simplified Monetization Model
```yaml
Ad Placements:
  Continue Opportunity:
    - Type: Rewarded video (30s)
    - Offer: One additional move
    - Limit: Once per run
    - Core revenue stream
    
Premium Option ($4.99 one-time):
  - Remove all ads
  - Unlimited continue videos
  - Double power-up earning rate
  - Extended queue preview (15 dots)
  - Priority leaderboard updates

Revenue Focus:
  - Watch ad to continue: Primary revenue
  - Premium upgrade: Secondary revenue
  - No power-up packs initially
  - No cosmetics initially
```

## 7. Success Metrics & KPIs

### 7.1 MVP Launch Metrics (6 Weeks)
- **Stability**: >99% crash-free sessions
- **Performance**: 30 FPS on 95% of devices
- **Single-Player Retention**: D1 >40%, D7 >20%, D30 >10%
- **Session Length**: Average >10 minutes
- **Downloads**: 5,000 in first month
- **Social Sharing**: >10% of players share scores

### 7.2 Growth Metrics (Months 3-6)
- **MAU**: 100,000 active users
- **DAU/MAU**: >30% (strong engagement)
- **Premium Conversion**: >5% of active users
- **Store Rating**: >4.3 stars
- **Organic Growth**: >20% month-over-month

### 7.3 Long-term Success (Year 1+)
- **Downloads**: 1M+ total
- **Retention**: Stable 25% D30
- **Revenue**: Profitable from ads/premium
- **Ranking**: Top 100 puzzle game
- **Community**: Active competitive scene

## 8. Development Plan

### 8.1 Simplified 6-Week MVP Roadmap
```yaml
WEEKS 1-2: Core Game Mechanics
  ✓ Directional gravity system
  ✓ 10-dot queue system
  ✓ Smart tile distribution
  ✓ Basic match detection
  ✓ Endless survival mode
  ✓ Basic UI and controls

WEEKS 3-4: Polish and Power-ups
  ✓ Basic power-ups (5 types)
  ✓ Power-up inventory system
  ✓ Shape accessibility
  ✓ Progressive hints
  ✓ Animation system
  ✓ Performance optimization

WEEK 5: Simple Leaderboards and Friend Codes
  ✓ Simple leaderboard system
  ✓ Friend code generation
  ✓ Add/remove friends
  ✓ Friends leaderboard view
  ✓ Basic score validation

WEEK 6: Social Sharing and Launch
  ✓ Social media sharing
  ✓ Ad integration (continue feature)
  ✓ Analytics setup
  ✓ Testing and bug fixes
  ✓ App store submission
```

### 8.2 6-Week Sprint Plan
**Sprint 1 (Weeks 1-2): Core Mechanics**
- Unity project architecture
- Grid and matching system
- Directional gravity core
- 10-dot queue implementation
- Basic tile graphics and UI

**Sprint 2 (Weeks 3-4): Polish & Power-ups**
- Smart distribution algorithm
- Anti-frustration system
- 5 basic power-ups
- Power-up inventory system
- Shape accessibility
- Animation system

**Sprint 3 (Weeks 5-6): Social & Launch**
- Simple leaderboard system
- Friend code system
- Social media sharing
- Ad integration (continue feature)
- Analytics setup
- Testing and submission

### 8.3 MVP Go-Live Criteria
```yaml
Technical Requirements:
  ✓ 30 FPS on 95% of test devices
  ✓ Crash rate <0.5%
  ✓ Load time <5 seconds
  ✓ All 5 power-ups working
  ✓ Queue system bug-free

Gameplay Requirements:
  ✓ 15+ minute average session
  ✓ Difficulty curve validated
  ✓ Tutorial hints effective
  ✓ No unwinnable states
  ✓ Shapes mode fully functional

Business Requirements:
  ✓ Ads displaying correctly
  ✓ Analytics capturing data
  ✓ App store assets ready
  ✓ Legal docs in place
  ✓ 50+ beta testers approved
```

## 9. Risk Assessment

### 9.1 Technical Risks
| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Directional gravity confusion | Medium | High | Extensive tutorial, visual indicators |
| Performance on old devices | Medium | Medium | Dynamic quality settings |
| Leaderboard cheating | High | Medium | Server validation, statistical analysis |

### 9.2 Market Risks
| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Player education challenge | High | High | Gradual tutorial, influencer demos |
| Crowded puzzle market | High | Medium | Unique mechanics, strong marketing |
| Retention below targets | Medium | High | Quick iteration, event content |

## 10. Testing Protocol

### 10.1 Family Testing (Weeks 1-2)
- 3-5 family members
- Various ages/skill levels
- Direct observation
- Focus: Queue understanding, power-up usage, control responsiveness

### 10.2 Beta Testing (Weeks 3-4)
- 50-100 external testers
- Puzzle game community recruitment
- Analytics on all interactions
- Focus: Retention, difficulty curve, monetization

### 10.3 Algorithm Validation
- A/B test directional vs traditional gravity
- Validate anti-frustration system
- Measure skill progression over time
- Ensure 50%+ score improvement with directional system

## 11. Final Design Lock

**CONFIRMED UNCHANGEABLE ELEMENTS:**
1. Core Mechanic: Directional gravity with player control
2. Queue System: 10 dots, vertical side display
3. Accessibility: Shape system (Square=Red, Circle=Blue, Triangle=Yellow)
4. Monetization: Watch ad to continue + premium option ($4.99)
5. Tutorial: Progressive hints, not forced
6. Technology: Unity 2022.3 LTS
7. MVP Timeline: 6 weeks to launch
8. Power-ups: Affect scoring, earned through play, unlimited use
9. Social: Friend codes + social media sharing only
10. Focus: 90% single-player experience with viral growth potential

---

**Document Control**
- Version: 1.3 COMPLETE PRODUCT VISION
- Status: APPROVED FOR DEVELOPMENT
- Next Phase: UML Diagramming
- Owner: Product Development Team

**END OF DOCUMENT**