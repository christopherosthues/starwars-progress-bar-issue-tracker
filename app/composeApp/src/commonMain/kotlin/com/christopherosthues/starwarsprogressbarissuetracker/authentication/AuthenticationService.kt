package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import io.ktor.client.HttpClient
import io.ktor.client.call.body
import io.ktor.client.plugins.ClientRequestException
import io.ktor.client.plugins.RedirectResponseException
import io.ktor.client.plugins.ServerResponseException
import io.ktor.client.request.post
import io.ktor.client.request.setBody
import io.ktor.client.statement.HttpResponse
import io.ktor.http.ContentType
import io.ktor.http.HttpStatusCode
import io.ktor.http.contentType
import io.ktor.http.formUrlEncode

class AuthenticationService(private val client: HttpClient) {
    suspend fun login(username: String, password: String) {
        val keycloakUrl = "https://your-keycloak-server/auth/realms/your-realm/protocol/openid-connect/token"

        try {
            val response: HttpResponse = client.post(keycloakUrl) {
                contentType(ContentType.Application.FormUrlEncoded)
                setBody(
                    listOf(
                        "client_id" to "your-client-id",
                        "grant_type" to "password",
                        "username" to username,
                        "password" to password
                    ).formUrlEncode()
                )
            }

            if (response.status == HttpStatusCode.OK) {
                response.body<TokenResponse>()
            }
        } catch (e: RedirectResponseException) {

        } catch (e: ClientRequestException) {

        } catch (e: ServerResponseException) {

        } catch (e: Exception) {

        }

    }

    suspend fun refreshToken() {

    }
}