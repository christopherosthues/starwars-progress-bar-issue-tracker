package com.christopherosthues.starwarsprogressbarissuetracker

import com.apollographql.apollo.ApolloClient
import com.christopherosthues.starwarsprogressbarissuetracker.authentication.AuthorizationInterceptor
import io.ktor.client.HttpClient

abstract class RepositoryBase(client: HttpClient) {
    protected val graphQLClient = ApolloClient.Builder()
        .serverUrl("http://localhost:8080/graphql")
        .addHttpInterceptor(AuthorizationInterceptor(client))
        .build()
}