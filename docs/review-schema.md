# Review Schema Notes

Two starter schemas are included:

- `schemas/project-snapshot.schema.json`
- `schemas/review-result.schema.json`

They define the normalized snapshot envelope and the strict review-result envelope needed before OpenAI integration is added. The current C# domain model is aligned to these envelopes using snake_case JSON serialization.
