# Architectural Kata: Command Queue and Abilities System for a Turn-Based Game

## Task
Implement a system that forms an event queue for a turn-based game. The queue should contain information about all interactions between units during combat.

## Description of Abilities
- **Bully:**  
  A unit that reacts to an attack by adding its own attack to the queue against the same target, provided the target is still alive.
  
- **Defender:**  
  A unit that redirects all attacks aimed at another unit to itself.

## Scenarios
1. **Double strike with one bully:**  
   - Unit A performs two strikes against B.  
   - Bully C inserts its attack after each strike by A.

2. **Double strike with a kill:**  
   - Unit A performs two strikes against B.  
   - B dies from the first strike, and subsequent commands targeting B are ignored.

3. **Two bullies:**  
   - A attacks B.  
   - Bullies C and D react to the attack, attacking B until it dies.

4. **Defender:**  
   - A attacks B.  
   - Defender E redirects A's attack to itself.

## Requirements
- The command queue is formed and processed correctly.
- Unit abilities (bullies, defender) operate independently.
- The code is easily extendable for adding new abilities.
