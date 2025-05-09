package com.christopherosthues.starwarsprogressbarissuetracker.authentication

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.PreferenceDataStoreFactory
import androidx.datastore.preferences.core.Preferences
import okio.Path.Companion.toPath

fun createDataStore(producePath: () -> String): DataStore<AuthenticationPreferences> {
    return PreferenceDataStoreFactory.createWithPath(produceFile = { producePath().toPath() })
}

internal const val dataStoreFileName = "issueTracker.preferences_pb"
