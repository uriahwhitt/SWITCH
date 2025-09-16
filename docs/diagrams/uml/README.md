# SWITCH UML Diagrams

This directory contains all UML diagrams for the SWITCH project, organized by type and phase.

## Diagram Types

### 01_class_diagrams/
Core class relationships and system architecture
- Core game objects and their relationships
- Power-up system class hierarchy
- Service layer architecture
- Data model relationships

### 02_sequence_diagrams/
Interaction flows and system communication
- Turn execution flow
- Power-up activation sequence
- Cascade resolution process
- Backend service interactions

### 03_state_diagrams/
State machines and game flow
- Game state machine
- Tile lifecycle states
- Power-up states
- Player progression states

### 04_activity_diagrams/
Process flows and algorithms
- Tile distribution algorithm
- Match detection process
- Queue generation flow
- Score calculation process

### 05_component_diagrams/
System architecture and component relationships
- Unity architecture overview
- Service integration diagram
- Module dependencies
- External system integration

### 06_use_case_diagrams/
User interactions and system boundaries
- Player interactions
- Admin system use cases
- Backend service use cases
- Analytics and reporting use cases

## Diagram Creation Phases

### Phase 1: Core Architecture (Before Sprint 1)
- [x] Core game objects class diagram
- [x] Turn execution sequence diagram
- [x] Game state machine diagram
- [x] Main game loop activity diagram
- [x] System context component diagram
- [x] System architecture component diagram
- [x] Deployment architecture component diagram

### Phase 2: Mechanics (Before Sprint 2)
- [x] Power-up system class diagram
- [x] Cascade resolution sequence diagram
- [x] Match detection sequence diagram
- [x] Power-up execution sequence diagram
- [x] Turn state machine diagram
- [x] Tutorial progressive flow activity diagram
- [x] Data flow activity diagram

### Phase 3: Systems (Before Sprint 3)
- [x] Unity architecture component diagram
- [x] Power-up activation sequence diagram
- [x] Core class relationships diagram
- [x] System context diagram
- [x] Deployment architecture diagram

### Phase 4: Backend (Before Sprint 4)
- [ ] Backend services class diagram
- [ ] System integration component diagram
- [ ] Leaderboard flow sequence diagram
- [ ] Social features use case diagram

## Diagram Standards

### Naming Convention
- Use descriptive names with underscores
- Include phase number prefix (01_, 02_, etc.)
- Use consistent terminology from PRD
- Include version numbers for major updates

### File Format
- Primary: Mermaid format (.mmd) for version control
- Export: PNG format for documentation
- Backup: PlantUML format (.puml) for compatibility

### Content Standards
- Include all relevant classes and relationships
- Show clear system boundaries
- Use consistent notation and symbols
- Include legends for complex diagrams

## Tools and Workflow

### Recommended Tools
- Mermaid for version-controlled diagrams
- PlantUML for complex diagrams
- Draw.io for collaborative editing
- Unity's built-in diagram tools for architecture

### Workflow
1. Create diagram in Mermaid format
2. Review with team
3. Export to PNG for documentation
4. Update related documentation
5. Commit to version control

### Review Process
- All diagrams must be reviewed before implementation
- Architecture diagrams require team approval
- Sequence diagrams must match implementation
- Update diagrams when architecture changes
