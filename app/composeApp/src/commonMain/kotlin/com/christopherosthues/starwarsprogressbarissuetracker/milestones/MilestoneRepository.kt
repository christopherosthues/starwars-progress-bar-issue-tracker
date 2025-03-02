package com.christopherosthues.starwarsprogressbarissuetracker.milestones

import com.christopherosthues.starwarsprogressbarissuetracker.RepositoryBase
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.MilestoneQuery
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.MilestonesQuery
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.type.UUID

class MilestoneRepository : RepositoryBase() {
    suspend fun getMilestones(): List<MilestonesQuery.Milestone> {
        val milestonesResponse = graphQLClient.query(MilestonesQuery()).execute()
        return milestonesResponse.dataAssertNoErrors.milestones
    }

    suspend fun getMilestone(id: UUID): MilestoneQuery.Milestone? {
        val milestoneResponse = graphQLClient.query(MilestoneQuery(id)).execute()
        return milestoneResponse.dataAssertNoErrors.milestone;
    }
}