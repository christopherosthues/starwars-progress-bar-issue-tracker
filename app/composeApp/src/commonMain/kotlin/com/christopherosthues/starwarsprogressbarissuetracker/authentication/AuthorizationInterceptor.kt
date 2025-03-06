package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import com.apollographql.apollo.api.http.HttpRequest
import com.apollographql.apollo.api.http.HttpResponse
import com.apollographql.apollo.network.http.HttpInterceptor
import com.apollographql.apollo.network.http.HttpInterceptorChain
import io.ktor.client.HttpClient
import io.ktor.client.request.post
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock

class AuthorizationInterceptor(private val client: HttpClient) : HttpInterceptor {
    private val mutex = Mutex()

    override suspend fun intercept(
        request: HttpRequest,
        chain: HttpInterceptorChain
    ): HttpResponse {
        var token = mutex.withLock {
            // TODO: get token with client
            client.post()
        }
        val response = chain.proceed(request.newBuilder().addHeader("Authorization", "Bearer $token").build())

        return if (response.statusCode == 401) {
            token = mutex.withLock {
                client.post()
            }
            chain.proceed(request.newBuilder().addHeader("Authorization", "Bearer $token").build())
        } else {
            response
        }
    }
}