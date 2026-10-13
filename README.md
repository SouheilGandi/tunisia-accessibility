# Tunisia Accessibility

A community-powered accessibility guide for wheelchair users, starting with a pilot in Greater Tunis. The home experience is a feed, with map and place search easy to reach.

## Product direction

- Collect dated, first-hand observations about access features and barriers at places.
- Keep unknown, conflicting, stale, and unverified information visible. A confirmation from someone who experienced the same condition is distinct from a helpfulness vote; the app will not reduce a place to one accessibility score.
- Let contributors suggest places and discuss observations. Venue responses remain separate from visitor reports, and moderation handles abuse and private information.
- Provide route previews only when the routing data supports them. A preview is not a guarantee that a journey is accessible; unknowns and conflicts must be shown.
- Start wheelchair-first, with French and English interface text. Other access needs and Arabic interface text are outside the initial scope.
- Keep the initial pilot free to operate without payment or credit-card details. Ads may be considered later, but must be clearly labeled, must not affect place or route ranking, and must not target people using precise location or mobility data.

## Technical direction

- **Android:** Kotlin and Jetpack Compose. Start with a native Android client; reconsider Kotlin Multiplatform only if another platform becomes a real need.
- **API:** ASP.NET Core 10.
- **Data:** PostgreSQL with PostGIS as the canonical application store.
- **Maps:** MapLibre with OpenFreeMap tiles. Use low-volume, user-initiated Nominatim place searches through the backend; do not use autocomplete. These public services have usage policies and no availability guarantee.
- **Routing:** Evaluate Valhalla for wheelchair route previews in the pilot and validate local route quality before relying on it.
- **Hosting:** Develop locally for now. Production hosting and photo storage are undecided and must fit the no-payment/no-card constraint.

## Repository status

The repository currently contains an ASP.NET API scaffold, account and authentication persistence, and a local PostGIS development setup. The accessibility feed, place reports, community moderation, maps, and route previews are product direction; they are not implemented yet.

See [GLOSSARY.md](GLOSSARY.md) for domain terms, [docs/adr/](docs/adr/) for accepted decisions, and [docs/research/](docs/research/) for technical comparisons.

## License

The code is licensed under [AGPL-3.0](LICENSE). The license for reusing contributed accessibility data is a separate decision and remains open.
