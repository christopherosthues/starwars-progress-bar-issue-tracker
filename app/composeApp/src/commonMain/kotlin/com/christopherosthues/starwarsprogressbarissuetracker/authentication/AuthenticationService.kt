package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import io.ktor.client.HttpClient
import io.ktor.client.engine.cio.CIO
import io.ktor.client.plugins.ClientRequestException
import io.ktor.client.plugins.RedirectResponseException
import io.ktor.client.plugins.ServerResponseException
import io.ktor.client.plugins.auth.Auth
import io.ktor.client.plugins.auth.providers.bearer
import io.ktor.client.request.get
import io.ktor.client.statement.HttpResponse

class AuthenticationService(private val client: HttpClient) {
    suspend fun login(username: String, password: String) {
        val client = HttpClient(CIO) {
            install(Auth) {
                bearer {

                }
            }
        }
        try {
            val response: HttpResponse = client.get("https://ktor.io/")
            println(response.status)
        } catch (e: RedirectResponseException) {

        } catch (e: ClientRequestException) {

        } catch (e: ServerResponseException) {

        } catch (e: Exception) {

        }

        client.close()
    }
}