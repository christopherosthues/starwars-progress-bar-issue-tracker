package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch

class LoginViewModel(private val authenticationService: AuthenticationService) : ViewModel() {
    fun login(username: String, password: String) {
        viewModelScope.launch { authenticationService.login(username, password) }
    }

    fun
}