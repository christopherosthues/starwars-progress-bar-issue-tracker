package com.christopherosthues.starwarsprogressbarissuetracker.authentication.dtos

import kotlinx.serialization.Serializable

@Serializable
data class LoginDto(val username: String, val password: String);
