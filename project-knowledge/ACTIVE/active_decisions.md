# SWITCH Active Decisions

## Current Decision Status
**Date**: January 2025
**Phase**: Project Setup

## Active Decisions

### 1. Project Structure
**Decision**: Follow SWITCH_Cursor_rules.md exactly
**Status**: Implemented
**Rationale**: Ensures consistency with planning team specifications
**Impact**: All directories and files follow established patterns

### 2. Technology Stack
**Decision**: Unity 2022.3 LTS
**Status**: Confirmed
**Rationale**: Best mobile performance and 2D game development tools
**Impact**: All development will use Unity ecosystem

### 3. Architecture Pattern
**Decision**: Event-driven with singleton GameManager
**Status**: Planned
**Rationale**: Loose coupling and familiar pattern for team
**Impact**: All systems will communicate through events

### 4. Performance Target
**Decision**: 60 FPS on mobile devices
**Status**: Established
**Rationale**: Required for competitive puzzle game
**Impact**: All code must be optimized for mobile performance

### 5. Directory Organization
**Decision**: Modular structure with clear separation
**Status**: Implemented
**Rationale**: Easy navigation and maintenance
**Impact**: All files organized by function and purpose

## Pending Decisions

### 1. Animation System
**Options**: DOTween vs Unity Animation vs Custom
**Status**: Under review
**Impact**: Affects performance and development speed

### 2. Audio System
**Options**: Unity AudioSource vs FMOD vs Wwise
**Status**: Under review
**Impact**: Affects audio quality and performance

### 3. Backend Service
**Options**: Firebase vs AWS vs Custom
**Status**: Under review
**Impact**: Affects development speed and scalability

### 4. Testing Framework
**Options**: Unity Test Framework vs Custom
**Status**: Under review
**Impact**: Affects code quality and maintenance

## Decision Process
1. Review options with team
2. Consider performance impact
3. Evaluate development speed
4. Check long-term maintainability
5. Make decision and document

## Next Decisions Needed
- Animation system selection
- Audio system selection
- Backend service selection
- Testing framework selection
