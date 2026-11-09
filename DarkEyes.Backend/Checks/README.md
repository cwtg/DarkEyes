# Checks

How an act finds out whether an attempt succeeds, without knowing the rules.

- **Purpose:** what a check is for; the rule module maps it to skill, attributes and modifiers.
- **Catalog:** every purpose the game's acts request, in one place: the contract the rule module covers.
- **Request and result:** the act's question (purpose, actor, target) and the generic answer (succeeded, failed, or not possible with the rule reason), whose meaning the caller decides.
- **Assessment:** the dice-free answer to whether a check may be attempted, and if not, why.
- **Check service:** the interface the rule module implements.
