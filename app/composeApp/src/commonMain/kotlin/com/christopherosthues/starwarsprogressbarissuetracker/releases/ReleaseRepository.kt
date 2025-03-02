package com.christopherosthues.starwarsprogressbarissuetracker.releases

import com.christopherosthues.starwarsprogressbarissuetracker.RepositoryBase
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.ReleaseQuery
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.ReleasesQuery
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.type.UUID

class ReleaseRepository : RepositoryBase() {
    suspend fun getReleases(): List<ReleasesQuery.Release> {
        val releasesResponse = graphQLClient.query(ReleasesQuery()).execute()
        return releasesResponse.dataAssertNoErrors.releases
    }

    suspend fun getRelease(id: UUID): ReleaseQuery.Release? {
        val releaseResponse = graphQLClient.query(ReleaseQuery(id)).execute()
        return releaseResponse.dataAssertNoErrors.release
    }
}