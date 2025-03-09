package com.christopherosthues.starwarsprogressbarissuetracker.authentication

data class AuthenticationPreferences(
    val accessToken: String,
    val refreshToken: String,
    val expiresIn: Int,
    val refreshExpiresIn: Int
)
