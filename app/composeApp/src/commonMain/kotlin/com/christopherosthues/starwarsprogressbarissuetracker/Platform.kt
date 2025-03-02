package com.christopherosthues.starwarsprogressbarissuetracker

interface Platform {
    val name: String
}

expect fun getPlatform(): Platform