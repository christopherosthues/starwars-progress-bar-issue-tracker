package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import kotlinx.serialization.Serializable

@Serializable
data class TokenResponse(
    val access_token: String,
    val refresh_token: String,
    val expires_in: Int
)