import com.apollographql.apollo.ApolloClient

abstract class RepositoryBase {
    protected val graphQLClient = ApolloClient.Builder()
        .serverUrl("http://localhost:8080/graphql")
        .build()
}