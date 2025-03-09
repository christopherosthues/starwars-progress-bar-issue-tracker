package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import kotlinx.serialization.Serializable

@Serializable
data class LoginDto(val userName: String, val password: String);
