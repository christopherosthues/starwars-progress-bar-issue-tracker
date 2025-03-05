package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import com.apollographql.apollo.api.http.HttpRequest
import com.apollographql.apollo.api.http.HttpResponse
import com.apollographql.apollo.network.http.HttpInterceptor
import com.apollographql.apollo.network.http.HttpInterceptorChain
import io.ktor.client.HttpClient

class AuthorizationInterceptor(private val client: HttpClient) : HttpInterceptor {
    override suspend fun intercept(
        request: HttpRequest,
        chain: HttpInterceptorChain
    ): HttpResponse {
        val token = ""
        // TODO: get token with client
        return chain.proceed(request.newBuilder().addHeader("Authorization", "Bearer $token").build())
    }
}