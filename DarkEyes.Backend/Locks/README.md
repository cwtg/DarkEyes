# Locks

The lock as part of an object: chests, doors and anything else that can be locked.

- **Lock:** whether something is locked. Everything that has a lock can be picked.
- **Unlocking:** the change from locked to unlocked and the event that reports it, shared by every act that unlocks (picking, a key, a spell, …).
- **Registration:** plugs the feature's handlers into the pipeline.
