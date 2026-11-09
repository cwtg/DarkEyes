# Randomness

Deterministic random numbers: the same state and seed give the same rolls on every runtime.

- **Generator:** our own PRNG (PCG32) as pure functions over a small, storable state.
- **Streams:** one stream per random consumer, derived from the master seed and the stream's ID, so consumers never shift each other's rolls.
- **Catalog:** the stream IDs of all random consumers, in one place.
- **Random source:** how game code draws numbers from a stream.
