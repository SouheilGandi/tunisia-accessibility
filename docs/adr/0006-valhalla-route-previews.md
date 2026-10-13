---
status: accepted
---

# Use Valhalla for pilot route previews

Evaluate Valhalla for wheelchair-aware route previews in the Greater Tunis pilot, while keeping the routing integration replaceable. Routes must expose unknown, unverified, and conflicting observations; they are not accessibility guarantees, and the first version will not provide turn-by-turn voice guidance. Validate route quality before relying on it, because Valhalla's wheelchair costing still has known limitations such as curb-height handling ([route options](https://github.com/valhalla/valhalla/blob/master/docs/docs/api/route/api-reference.md), [implementation note](https://github.com/valhalla/valhalla/blob/master/src/sif/pedestriancost.cc)).
