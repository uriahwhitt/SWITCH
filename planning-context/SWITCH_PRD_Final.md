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
- Heat System Engagement: Average heat level 3-5, max heat achieved 8+ by skilled players
- Power Orb Strategy: 60%+ of orbs collected at correct edge, strategic timing decisions
- Momentum Retention: Players maintain heat levels for 3+ consecutive turns
- Turn Execution Flow: 95%+ of players understand double-tap selection system
- Blocking Block Management: Players successfully clear 80%+ of blocking blocks
- Extended Queue System: Players utilize 15-tile look-ahead for strategic planning

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

### 4.2 Extended Block Queue System (15-Tile System)
**Enhanced Queue Mechanics with Anti-Frustration**

**Visual Layout:**
```
Left Side of Screen:
┌─┐
│●│ <- Top (10th position) - Newest Visible
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

Hidden Buffer (5 tiles):
┌─┐
│●│ <- Buffer 5
│●│ <- Buffer 4
│●│ <- Buffer 3
│●│ <- Buffer 2
│●│ <- Buffer 1
└─┘
```

**CORRECTED Queue Flow Sequence:**
1. Player double-taps to select two adjacent tiles
2. System caches selection data (positions + direction)
3. Swap animation previews the move
4. Match validation occurs BEFORE gravity calculation
5. If NO match: Tiles animate back, return to step 1
6. If YES match: Extract gravity from cached selection
7. Clear matched tiles (3 min, 5 max)
8. Apply gravity in calculated direction
9. Fill gaps from queue (clockwise: Top→Right→Bottom→Left)
10. Check for cascades and process all matches
11. Refill queue to 15 tiles (10 visible + 5 buffer)
12. Turn complete, await player input

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

### 4.4 Special Tile Systems

#### 4.4.1 Blocking Blocks (Obstacles)
**Strategic Obstacles System**
- **Appearance**: Stone/concrete texture (clearly different from regular tiles)
- **Behavior**: Can be swapped with regular tiles, but only if the regular tile creates a match
- **Cannot be matched**: Blocking blocks cannot be part of matches themselves
- **Removal**: Must reach board edge to be removed
- **Progressive Introduction**:
  - Tutorial: 1 block for demonstration
  - First 5 minutes: 0% spawn rate
  - After 5 minutes: 2% spawn rate
  - Every 2 minutes: +1% (max 10%)
- **Strategic Purpose**: Forces multi-turn planning and creates board obstacles

#### 4.4.2 Power Orbs (Center-Spawn System)
**High-Value Objective System**
- **Spawn Location**: 4 center cells (3,3), (4,3), (3,4), (4,4)
- **Trigger**: When center cell is cleared by match
- **Spawn Chance**: 5% base, increasing over time (max 15%)
- **Timing**: IMMEDIATE spawn before gravity/cascade
- **Orb Properties**:
  - **Appearance**: Glowing colored orbs (not tiles)
  - **Colors/Targets**: Blue→TOP, Green→RIGHT, Yellow→BOTTOM, Purple→LEFT
  - **Scoring**: Base 5,000 points + 500 per turn survived
  - **Cascade Multipliers**: Apply if collected during cascade
  - **Success**: Only scores if reaching correct edge
  - **Failure**: Falls off wrong edge = lost (no points)
- **Visual Indicators**: Matching colored edge glow/border, pulsing glow effect

### 4.5 Endless Survival Mode (Primary Game Mode)
**Enhanced Core Gameplay Loop**
1. View extended block queue (10 visible + 5 buffer)
2. Analyze board state including special tiles
3. Double-tap to select two adjacent tiles
4. System validates match before gravity calculation
5. If valid: Extract gravity from cached selection
6. Clear matches and apply gravity
7. Fill gaps clockwise (Top→Right→Bottom→Left)
8. Process cascades and special tile interactions
9. Build momentum through skillful play
10. Collect power orbs for massive score boosts
11. Manage blocking blocks strategically
12. Speed gradually increases
13. Game ends when no moves available

### 4.6 Momentum-Based Scoring System
**Revolutionary Heat System**
- Players build "heat" through complex matches and cascades
- Heat creates score multipliers (1.0x to 10.0x)
- Automatic decay prevents coasting (-1.0 heat per turn)
- Rewards sustained skillful play over lucky single moves

**Heat Generation Mechanics**
- Match-3: 0 heat (maintains current level minus decay)
- Match-4: +1.0 heat (good momentum boost)
- Match-5: +2.0 heat (excellent momentum boost)
- Cascades: +0.5 heat per level (cumulative)
- L-shape patterns: +1.0 heat (pattern bonus)
- Cross patterns: +1.5 heat (rare pattern bonus)

**Heat Level Categories**
- **Cold (0-2 heat)**: 1.0x-2.8x multiplier, blue visual theme
- **Warm (3-4 heat)**: 3.7x-4.6x multiplier, yellow visual theme
- **Hot (5-7 heat)**: 5.5x-7.3x multiplier, orange visual theme
- **Blazing (8-9 heat)**: 8.2x-9.1x multiplier, red visual theme
- **Inferno (10 heat)**: 10.0x multiplier, white-hot visual theme

**Position-Based Scoring**
- **Edge Positions**: 1x multiplier (outer ring)
- **Transition Positions**: 2x multiplier (middle ring)
- **Center Positions**: 3x multiplier (center 4 cells)
- **Pattern Bonuses**: L-shape (+50 points), Cross (+100 points)

**Power Orb Integration**
- Spawn in center when center cells are cleared
- Move toward specific colored edges over time
- **Instant Max Heat**: Collecting at correct edge = 10.0 heat immediately
- **Age-Based Scoring**: Base 5,000 points + 500 per turn survived
- **Strategic Timing**: Risk/reward decisions about when to collect
- **Wrong Edge**: 0 points, orb is lost

**Dynamic Audio System**
- **Layered Music**: Base, Rhythm, Melody, Climax layers
- **Tempo Changes**: 120-180 BPM based on heat level
- **Heartbeat Effect**: Accelerates with heat level
- **Sound Effects**: Heat up, cool down, power orb explosion

**Visual Heat System**
- **Heat Meter**: Color-coded bar with multiplier display
- **Particle Effects**: Heat, flame, and inferno particles
- **Screen Effects**: Edge glow and pulsing effects
- **Color Transitions**: Smooth color changes between heat levels

**Difficulty Progression**
- Speed increase every 60 seconds
- Pressure intensity grows over time
- Special blocks appear at milestones
- No artificial difficulty walls

### 4.7 Daily Challenges & Events
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

### 4.8 Progression & Engagement Systems
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

### 4.9 Simple Leaderboard System
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

### 4.10 Social Sharing Features
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

### 4.11 Intelligent Tile Distribution System
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

### 4.12 Power-Up System
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

### 4.13 Accessibility System
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

### 4.14 Tutorial System
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

### 5.4 Screen Layout Architecture - FINAL DESIGN
```
Portrait Mode (Mobile Primary):
┌────────────────────────────┐
│ 1,247,350      [☰]         │ 4%   Score & Menu
├────────────────────────────┤
│🔥 ██████░░░░ x6.5 HEAT!    │ 5%   Heat Meter
├──┬─────────────────────────┤
│Q │                         │
│u │    8x8 Game Grid        │ 70%  Main Game Area
│e │   (with edge glows)     │
│10│                         │
├──┴─────────────────────────┤
│ [🔥][💣][⚡]   CASCADE x3!  │ 10%  Powers & Feedback
├────────────────────────────┤
│    [Ad Banner 320x50]      │ 5%   Monetization
└────────────────────────────┘
Percentage Breakdown:
- 4% - Score & Menu Bar
- 5% - Heat Meter Bar  
- 70% - Game Grid & Queue
- 10% - Power-ups & Active Feedback
- 5% - Ad Banner Space
- 6% - Padding/margins

Key UI Components:
1. TopBarUI - Score display with animated counter and hamburger menu
2. HeatMeterUI - Visual heat meter with color transitions and particle effects
3. GameAreaUI - 8x8 grid with queue panel and edge glow indicators
4. PowerFeedbackUI - Power-up slots with dynamic feedback display
5. AdBannerUI - Monetization banner with fallback content
6. MenuOverlayUI - Pause menu with full game options
7. UIScaler - Responsive design for different device types
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
  ✓ Directional gravity system with swap caching
  ✓ Extended 15-tile queue system (10 visible + 5 buffer)
  ✓ Smart tile distribution with anti-frustration
  ✓ Basic match detection
  ✓ Endless survival mode
  ✓ Basic UI and controls
  ✓ Swap validation before gravity calculation

WEEKS 3-4: Special Tiles and Polish
  ✓ Blocking blocks system with progressive introduction
  ✓ Power orbs system with center spawning
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
- Directional gravity core with swap caching
- Extended 15-tile queue implementation
- Basic tile graphics and UI
- Swap validation system

**Sprint 2 (Weeks 3-4): Special Tiles & Polish**
- Smart distribution algorithm
- Anti-frustration system
- Blocking blocks system
- Power orbs system
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
1. Core Mechanic: Directional gravity with player control and swap caching
2. Queue System: Extended 15-tile system (10 visible + 5 buffer)
3. Special Tiles: Blocking blocks and power orbs systems
4. Turn Execution: Double-tap selection with match validation before gravity
5. Accessibility: Shape system (Square=Red, Circle=Blue, Triangle=Yellow)
6. Monetization: Watch ad to continue + premium option ($4.99)
7. Tutorial: Progressive hints, not forced
8. Technology: Unity 2022.3 LTS
9. MVP Timeline: 6 weeks to launch
10. Power-ups: Affect scoring, earned through play, unlimited use
11. Social: Friend codes + social media sharing only
12. Focus: 90% single-player experience with viral growth potential

---

**Document Control**
- Version: 1.3 COMPLETE PRODUCT VISION
- Status: APPROVED FOR DEVELOPMENT
- Next Phase: UML Diagramming
- Owner: Product Development Team

**END OF DOCUMENT**