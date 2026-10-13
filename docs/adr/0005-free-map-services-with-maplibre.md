---
status: accepted
---

# Use MapLibre with free OpenStreetMap-based map services

Use the MapLibre Android renderer with OpenFreeMap tiles and low-volume, user-initiated Nominatim searches; do not use Nominatim autocomplete. Send Nominatim lookups through the backend, where requests can be rate-limited and cached in line with its policy. This meets the no-payment/no-card constraint, with the accepted trade-off that these public services have usage constraints and no availability guarantee. Keep place suggestions and manual map-pin selection available when search misses a place. See the [OpenFreeMap terms](https://openfreemap.org/tos/) and [Nominatim usage policy](https://operations.osmfoundation.org/policies/nominatim/).
