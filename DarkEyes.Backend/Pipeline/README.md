# Pipeline

How changes get into the state, and how the outside world learns about them.

- **Entry point:** the one object that owns the committed state and accepts actions; its builder wires up all handlers and installs the rule module's services.
- **Messages:** actions (an actor's attempt, which the rules may refuse), effects (a decreed change), events (what happened, for presentation).
- **Handlers and contexts:** action handlers decide and delegate, effect handlers change the state; both can request checks; each kind gets a context that allows exactly what it may do.
- **Routing:** routers know which handler belongs to which message, and nothing else.
- **Chains and transactions:** a chain is one run of the pipeline, a root and everything that follows from it; all its changes go into a transaction that is committed as a whole or not at all.
- **Results:** what a dispatch returns — either a commit as ordered steps, each an event plus the patches it caused, or a rejection: a reason code with arguments, and nothing changed.
