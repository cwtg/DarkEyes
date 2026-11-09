# State

The game's single source of truth: one immutable state tree.

- **The tree:** everything that defines a game in progress lives in one root, which is replaced as a whole, never modified.
- **Entities:** everything in the world is an entity, an identity plus a set of components (plain data).
- **Component keys:** components are stored under stable names; typed keys are the only way to read and write them.
- **Random streams:** the master seed and the state of every PRNG stream used so far; hidden from the frontend.
