# Backend and database choice for the accessibility map

**Research date:** 13 October 2026  
**Scope:** Compare credible backend and database families for a low-cost Greater Tunis pilot with a native Kotlin/Compose Android client, community place and barrier reports, evidence and freshness, moderation, photo uploads, and wheelchair route previews based on OpenStreetMap (OSM).

## Recommendation

**Decision status (13 October 2026):** This comparison is research, not the deployment plan. The project has accepted ASP.NET Core and PostgreSQL/PostGIS for the API and canonical data store (see ADRs [0002](../adr/0002-retain-aspnet-api.md) and [0003](../adr/0003-postgresql-postgis.md)). Production hosting, photo storage, and paid services are deferred because the maintainer cannot provide payment or card details; develop locally until the pilot demonstrates value (see ADR [0008](../adr/0008-defer-production-hosting.md)). The managed database and object storage suggestions below are options to revisit later, not current commitments.

For a future production deployment, a **modular monolith HTTP API** and **PostgreSQL with PostGIS** remain a sound architecture. Managed hosting and separate object storage are options to reconsider only if they meet the no-payment/no-card constraint at that time. The accepted project choice is to retain the existing ASP.NET Core API and PostgreSQL/PostGIS stack and develop locally for now. Other backend families remain technically capable, but changing to them would discard existing work without a current need. No backend language makes wheelchair route data reliable by itself.

Use a separate object store for photo bytes and keep metadata, ownership, moderation state, and links in PostgreSQL. Treat the routing engine and its routable OSM graph as a separate service/component from the application API and its transactional database. Do not build graph routing by asking a general-purpose database to substitute for a routing engine.

This recommendation is based on functional fit and likely pilot simplicity. It is **not** a provider price comparison: exact cost depends on hosting region, storage, egress, image volume, backups, and free-tier terms, which should be checked against current provider quotes before deployment.

## What the data model needs

The app needs relational links among places, contributors, individual dated observations, evidence/media, moderation actions, and independent confirmations or conflicting reports. It also needs point/line/polygon operations for nearby places and barriers, spatial filtering, and possibly querying accessibility reports along route geometry. Reports need to retain provenance and history rather than be overwritten into one score. These are a natural fit for relational constraints plus spatial types and indexes.

The routing use case adds a different need: a pedestrian graph with wheelchair-specific edge costs and restrictions. A route engine such as Valhalla consumes OSM-derived graph data and returns a route; the app should store its own observations and route annotations independently. A route preview should expose unknown or weakly supported segments and avoid claiming accessibility when the evidence is insufficient.

## Backend options

| Backend family | Fit for this project | Main trade-offs |
|---|---|---|
| **ASP.NET Core / C#** | Strong. HTTP APIs, authentication/authorization, validation, background jobs, image-upload coordination, and PostgreSQL integration are all conventional. ASP.NET Core recommends Minimal APIs for new APIs and documents support for complex routes and authorization. Npgsql’s EF Core provider supports NetTopologySuite spatial mapping. | Existing repo already has API and persistence projects and Npgsql spatial dependencies. Maintaining C# is a separate skill choice; it does not satisfy a Kotlin-only learning goal. |
| **Node.js / TypeScript** | Strong. Good fit for JSON APIs, uploads, notifications, and real-time community features. | No material geospatial advantage over the alternatives when using PostGIS. It may be attractive if a TypeScript web client and shared schemas become important. Use a supported LTS release in production; Node’s official policy distinguishes Current and LTS lines. |
| **Java / Spring Boot** | Strong and mature for structured APIs, security, and background processing; supports embedded HTTP servers and both MVC and reactive styles. | More framework surface than a small pilot may need. It is relevant if the maintainers prefer JVM tooling or expect a larger organization/backend. |
| **Kotlin / Ktor** | Strong enough for the same API, and directly extends the Android Kotlin learning path. Official Ktor docs cover REST JSON APIs and database integration. | Server-side Kotlin is an additional learning track, and there is no shared Android/server code benefit for ordinary domain logic that should remain behind an API. Existing C# scaffold would be replaced or abandoned. A lighter framework is not automatically a lower-maintenance system. |
| **Go** | Strong for a compact HTTP service and operationally lean deployments. | No special advantage for relational geospatial querying; PostGIS still does that work. Choose it when the maintainers know Go or value its deployment model, not because routing needs Go. |
| **Python (FastAPI/Django)** | Strong for an API, moderation/admin tooling, and possible data-ingestion or analysis workflows. | Python can serve the product API, but GIS/routing analytics can also run as separate jobs. Framework and team expertise matter more than a presumed AI advantage; the MVP does not need an AI backend. |
| **Managed BaaS (e.g. Supabase/Firebase)** | Can reduce initial operations: managed auth, database, storage, and generated APIs are useful for a small pilot. Supabase uses PostgreSQL and documents PostGIS and object storage. | Evaluate authorization carefully: community reports, private account data, business submissions, moderator actions, and photo access have different rules. A BaaS does not remove the need for a trusted server boundary or careful data model. Firebase/Firestore’s documented geohash method uses multiple range queries and client-side false-positive filtering; native Firestore geospatial queries are documented as an Enterprise feature in preview. This makes Firestore less attractive as the canonical spatial/report database here. |
| **Serverless functions / edge functions** | Useful for isolated tasks such as image processing, notifications, scheduled recheck reminders, or low-volume endpoints. | Not a database choice and not necessarily a whole backend architecture. Stateful workflows, moderation/audit, and routing integration can become fragmented if every domain operation is a separate function. |

### Backend decision

There is no universal backend winner. With this repository and pilot scope, start with **ASP.NET Core as a modular monolith** unless the maintainer explicitly wants to learn Kotlin server development too. Keep modules organized around places, observations, media, moderation, accounts, and routing integration; deploy as one API initially. Invoke the routing engine through a stable adapter/API so it can be replaced independently. If choosing afresh solely to maximize Kotlin practice, Ktor is credible, but it is a deliberate language/learning trade-off rather than a technical requirement of the app.

Avoid microservices at pilot scale. Separate deployable components only where operational or ownership boundaries justify them: the routing engine and object storage are already natural external components.

## Database options

| Database family | Spatial capability and fit | Assessment |
|---|---|---|
| **PostgreSQL + PostGIS** | PostGIS provides spatial types, spatial predicates/processing, and GiST spatial indexes; its `geography` type represents latitude/longitude data on the earth. PostgreSQL also gives transactions, foreign keys, and ordinary relational queries for provenance and moderation. | **Best fit.** One canonical store can represent reports, places, media metadata, moderation history, and spatial geometry without splitting relational and GIS data. PostGIS is a first-class ecosystem used by GIS and OSM tools. |
| **MySQL / MariaDB GIS** | MySQL includes spatial types and R-tree spatial indexes; indexed columns have constraints such as `NOT NULL` and SRID requirements. | Viable for basic point proximity and common spatial workloads, particularly where MySQL is already operated. PostGIS has broader GIS-oriented functionality and is the safer default for evolving route geometry and spatial-analysis needs. |
| **SQL Server spatial** | SQL Server provides `geometry` and round-earth `geography` types plus spatial indexes. Spatial indexes require a clustered primary key and use tessellation schemes. | Capable, especially in Microsoft-heavy environments. Less compelling than PostgreSQL/PostGIS for an OSM/GIS-centered, cost-conscious pilot unless there is an existing SQL Server operations advantage. |
| **SQLite + SpatiaLite** | SQLite is an embedded database; SpatiaLite adds spatial SQL capabilities. | Excellent for offline/local data, prototypes, or a future Android cache. Not the recommended shared authoritative server database for multi-user writes, moderation, and centralized backups. |
| **MongoDB** | Supports GeoJSON geospatial indexes and spherical queries (`2dsphere`). | Technically possible for point search, but the app’s strongly related observations, authorship, evidence, moderation, and audit history benefit from relational constraints and transactions. It is a weaker default for this domain. |
| **Firestore / document BaaS** | Geohash-based searches approximate radius queries, require multiple range queries, then filter false positives. Native geospatial query docs currently describe an Enterprise preview feature. | Fast to prototype and operationally convenient, but its documented spatial approach adds complexity/cost for repeated queries and offers a weaker fit for rich spatial relations and connected route geometry. Do not select it as canonical storage solely because the Android client is Kotlin. |
| **Graph databases (e.g. Neo4j)** | Useful for graph traversal and relationships. | Could model a routable network, but it does not replace OSM ingestion, wheelchair edge-cost policy, map matching, route data QA, or a specialized route engine. Avoid operating both a graph DB and relational DB for the pilot unless a measured need appears. |
| **Search engines (Elasticsearch/OpenSearch)** | Useful for full-text search and ranking at scale. | Not a source of truth for transactional observations or geospatial audit history. PostgreSQL full-text/trigram search is enough to start; add a search service only after measured search needs justify replication and operations. |
| **Distributed/NewSQL systems (CockroachDB, YugabyteDB, etc.)** | Provide distributed SQL or horizontal scaling features; geospatial feature depth varies by product/version. | Adds operational or compatibility complexity without a pilot requirement for multi-region write scale. Revisit only with concrete scale/availability requirements and proof that required PostGIS functions are supported. |

### Database decision

Choose **PostgreSQL + PostGIS** for the canonical server data. It handles spatial and relational needs together and keeps reports queryable near places and route geometries. Keep route graph processing in Valhalla (or a comparable routing engine), not in SQL. Keep photos in object storage, not database blobs. SQLite can later support offline caching, but it should not be confused with the server source of truth.

## Cost-conscious pilot architecture

1. Android app talks to one HTTPS API.
2. API uses PostgreSQL/PostGIS for accounts, place records, observation history, status/freshness, business submissions, moderation events, and photo metadata.
3. Photos upload to object storage using short-lived signed upload/download authorization; the database stores references and moderation/visibility state.
4. API calls a separately deployable Valhalla instance or hosted routing endpoint. Route responses are annotated with the app’s own barrier reports and uncertainty; routing graph and user evidence remain separate datasets.
5. Start with one backend deployment and one managed PostgreSQL instance in a chosen region, with automated backups. Add caches, queues, search indexes, or additional services only when usage or operational evidence warrants them.

The best low-cost option depends on the hosting offers available in the desired region, data-residency needs, expected photo volume, and the operator’s willingness to maintain services. Compare provider quotes only after deciding whether managed Postgres and object storage are available in a suitable region. Self-hosting may reduce invoice cost but transfers backup, upgrade, availability, and security work to the project.

## Evidence and primary sources

- Microsoft recommends Minimal APIs for new ASP.NET Core APIs and notes support for complex routes and authorization: [ASP.NET Core API overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-10.0).
- Npgsql documents EF Core spatial mapping via NetTopologySuite: [Npgsql spatial mapping](https://www.npgsql.org/efcore/mapping/nts.html).
- Node.js documents release lines and recommends production use of Active or Maintenance LTS: [Node.js release schedule](https://nodejs.org/en/about/previous-releases).
- Spring Boot documents embedded HTTP servers and MVC/reactive web approaches: [Spring Boot web reference](https://docs.spring.io/spring-boot/reference/web/index.html).
- Ktor documents REST APIs, JSON serialization, routing, and database integration paths: [Ktor REST API tutorial](https://ktor.io/docs/server-create-restful-apis.html), [Ktor project guide](https://ktor.io/docs/server-create-a-new-project.html).
- PostGIS documents GIS processing, geography types, and spatial index support: [PostGIS manual](https://postgis.net/docs/).
- MySQL documents spatial types and spatial indexes: [spatial data types](https://dev.mysql.com/doc/refman/8.4/en/spatial-type-overview.html), [spatial indexes](https://dev.mysql.com/doc/refman/8.4/en/creating-spatial-indexes.html).
- Microsoft documents SQL Server `geography` and the spatial-index requirements: [geography type](https://learn.microsoft.com/en-us/sql/t-sql/spatial-geography/spatial-types-geography?view=sql-server-ver17), [spatial index](https://learn.microsoft.com/en-us/sql/t-sql/statements/create-spatial-index-transact-sql?view=sql-server-ver17).
- MongoDB documents spherical `2dsphere` indexes and geospatial queries: [geospatial queries](https://www.mongodb.com/docs/manual/geospatial-queries/), [geospatial indexes](https://www.mongodb.com/docs/manual/core/indexes/index-types/index-geospatial/).
- Firebase documents geohash query limitations and Firestore Enterprise geospatial query feature status: [geohash queries](https://firebase.google.com/docs/firestore/solutions/geoqueries), [Enterprise geospatial query](https://firebase.google.com/docs/firestore/enterprise/geospatial-query).
- Supabase documents PostGIS support and storage capabilities: [PostGIS](https://supabase.com/docs/guides/database/extensions/postgis), [Storage](https://supabase.com/docs/guides/storage).

## Repository observation

At research time, `backend/src/Tunac.Api` is an ASP.NET Core project and `backend/src/Tunac.Infrastructure` references EF Core, Npgsql, and the Npgsql NetTopologySuite provider. Its `TunacDbContext` declares PostGIS plus `pg_trgm`, `unaccent`, and `citext` extensions, but current entities shown there are user/authentication records rather than accessibility-place/report entities. This supports an incremental implementation path, but should not be mistaken for an already implemented map backend.
