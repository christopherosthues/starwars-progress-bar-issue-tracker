package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import com.christopherosthues.starwarsprogressbarissuetracker.authentication.dtos.LoginDto
import com.christopherosthues.starwarsprogressbarissuetracker.authentication.dtos.RefreshTokenDto
import com.christopherosthues.starwarsprogressbarissuetracker.authentication.dtos.RegistrationDto
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
import kotlinx.coroutines.flow.lastOrNull

class AuthenticationService(private val client: HttpClient, private val preferencesRepository: AuthenticationPreferencesRepository) {
    suspend fun login(username: String, password: String) {
        val loginUrl = "https://localhost:8080/login"

        try {
            val response: HttpResponse = client.post(loginUrl) {
                contentType(ContentType.Application.Json)
                setBody(
                    LoginDto(username, password)
                )
            }

            if (response.status == HttpStatusCode.OK) {
                val tokenResponse = response.body<TokenResponse>()
                preferencesRepository.updateAccessPreferences(
                    tokenResponse.accessToken,
                    tokenResponse.refreshToken,
                    tokenResponse.expiresIn,
                    tokenResponse.refreshExpiresIn
                )
            }
        } catch (e: RedirectResponseException) {

        } catch (e: ClientRequestException) {

        } catch (e: ServerResponseException) {

        } catch (e: Exception) {

        }
    }

    suspend fun refreshToken() {
        val refreshUrl = "https://localhost:8080/refresh"

        try {
            val authenticationPreferences =
                preferencesRepository.authenticationPreferencesFlow.lastOrNull()
                    ?: // TODO: logout / navigate to login screen
                    return

            val response: HttpResponse = client.post(refreshUrl) {
                contentType(ContentType.Application.Json)
                setBody(
                    RefreshTokenDto(authenticationPreferences.refreshToken)
                )
            }

            if (response.status == HttpStatusCode.OK) {
                val tokenResponse = response.body<TokenResponse>()
                preferencesRepository.updateAccessPreferences(
                    tokenResponse.accessToken,
                    tokenResponse.refreshToken,
                    tokenResponse.expiresIn,
                    tokenResponse.refreshExpiresIn
                )
            }
        } catch (e: RedirectResponseException) {

        } catch (e: ClientRequestException) {

        } catch (e: ServerResponseException) {

        } catch (e: Exception) {

        }
    }

    suspend fun registerUser(
        username: String,
        email: String,
        password: String,
        firstName: String,
        lastName: String
    ) {
        val registerUrl = "https://localhost:8080/register"

        try {
            val response: HttpResponse = client.post(registerUrl) {
                contentType(ContentType.Application.Json)
                setBody(
                    RegistrationDto(username, email, password, firstName, lastName)
                )
            }

            if (response.status == HttpStatusCode.OK) {
                // TODO: redirect to login page
            } else {
                // TODO: notify user could not be registered
            }
        } catch (e: RedirectResponseException) {

        } catch (e: ClientRequestException) {

        } catch (e: ServerResponseException) {

        } catch (e: Exception) {

        }
    }
}