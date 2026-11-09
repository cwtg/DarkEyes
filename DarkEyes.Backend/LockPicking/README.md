# Lock picking

The act of picking a lock: a skill application whose outcome the rule module decides.

- **Picking:** an actor's attempt to pick a lock; the backend validates it, a check decides whether it succeeds, and a success unlocks without opening; a check that isn't possible is reported as such.
- **Rejections:** the reasons why an attempt to pick a lock can be refused.
- **Registration:** plugs the act into the pipeline.
