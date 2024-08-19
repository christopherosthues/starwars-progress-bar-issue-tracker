package labels

import RepositoryBase
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.LabelQuery
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.LabelsQuery
import com.christopherosthues.starwarsprogressbarissuetracker.graphql.type.UUID

class LabelRepository : RepositoryBase() {
    suspend fun getLabels(): List<LabelsQuery.Label> {
        val labelsResponse = graphQLClient.query(LabelsQuery()).execute()
        return labelsResponse.dataAssertNoErrors.labels
    }

    suspend fun getLabel(id: UUID): LabelQuery.Label? {
        val labelResponse = graphQLClient.query(LabelQuery(id)).execute()
        return labelResponse.dataAssertNoErrors.label
    }
}