# 1. Use Markdown Anywhere Architecture Decision Records

Date: 2026-07-23

## Status

Accepted

## Context

We need to record the architectural decisions made in this project. As the project relies heavily on metaprogramming (Source Generators) and code emission, the design choices are often non-obvious and highly impactful to compile-time performance and runtime behavior.

We need a way to document these decisions that is lightweight, version-controlled alongside the code, and easily accessible to all contributors.

## Decision

We will use Markdown Anywhere Architecture Decision Records (MADR) to document architectural decisions. 
These records will be stored in the `docs/adr/` directory in the repository root.
The format will follow a simplified template containing: Title, Date, Status, Context, Decision, and Consequences.

## Consequences

* **Positive:** Decisions are explicitly documented and versioned with the code they affect.
* **Positive:** New contributors can read the history of *why* things were built a certain way, reducing redundant questions (e.g., "Why not just use Reflection?").
* **Negative:** It requires discipline from maintainers to write an ADR for every significant architectural change before or during implementation.
