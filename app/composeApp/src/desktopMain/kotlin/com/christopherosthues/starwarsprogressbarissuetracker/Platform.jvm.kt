package com.christopherosthues.starwarsprogressbarissuetracker

class JVMPlatform: Platform {
    override val name: String = "${System.getProperty("os.name")} (${System.getProperty("os.version")}) running Java ${System.getProperty("java.version")}"
}

actual fun getPlatform(): Platform = JVMPlatform()