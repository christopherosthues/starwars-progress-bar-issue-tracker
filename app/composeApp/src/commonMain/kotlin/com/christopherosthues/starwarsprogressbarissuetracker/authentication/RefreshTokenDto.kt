package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import kotlinx.serialization.Serializable

@Serializable
data class RefreshTokenDto(val refreshToken: String)
