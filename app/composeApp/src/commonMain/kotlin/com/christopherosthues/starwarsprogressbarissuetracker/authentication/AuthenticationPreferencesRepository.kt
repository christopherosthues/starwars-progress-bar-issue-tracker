package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import androidx.datastore.core.DataStore
import androidx.datastore.core.IOException
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.emptyPreferences
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.catch
import kotlinx.coroutines.flow.map

class AuthenticationPreferencesRepository(private val dataStore: DataStore<Preferences>) {
    val authenticationPreferencesFlow: Flow<AuthenticationPreferences> = dataStore.data
        .catch {
            if (it is IOException) {
                emit(emptyPreferences())
            } else {
                throw it
            }
        }.map {
            val accessToken = it[AuthenticationPreferencesKeys.ACCESS_TOKEN] ?: ""
            val refreshToken = it[AuthenticationPreferencesKeys.REFRESH_TOKEN] ?: ""
            val expiresIn = it[AuthenticationPreferencesKeys.EXPIRES_IN] ?: 0
            val refreshExpiresIn = it[AuthenticationPreferencesKeys.REFRESH_EXPIRES_IN] ?: 0
            AuthenticationPreferences(accessToken, refreshToken, expiresIn, refreshExpiresIn)
        }

    suspend fun updateAccessPreferences(
        accessToken: String,
        refreshToken: String,
        expiresIn: Int,
        refreshExpiresIn: Int
    ) {
        dataStore.edit {
            it[AuthenticationPreferencesKeys.ACCESS_TOKEN] = accessToken
            it[AuthenticationPreferencesKeys.REFRESH_TOKEN] = refreshToken
            it[AuthenticationPreferencesKeys.EXPIRES_IN] = expiresIn
            it[AuthenticationPreferencesKeys.REFRESH_EXPIRES_IN] = refreshExpiresIn
        }
    }
}