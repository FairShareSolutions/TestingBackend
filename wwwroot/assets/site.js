"use strict";

document.addEventListener("DOMContentLoaded", function () {
    // Mobile navigation
    const toggle = document.querySelector(".mobile-toggle");
    const links = document.querySelector(".links");

    if (toggle && links) {
        toggle.addEventListener("click", function () {
            const isOpen = links.classList.toggle("open");

            toggle.setAttribute("aria-expanded", String(isOpen));
            toggle.setAttribute(
                "aria-label",
                isOpen ? "Close navigation" : "Open navigation"
            );

            toggle.textContent = isOpen ? "✕" : "☰";
        });

        // Close the mobile menu after selecting a navigation link.
        links.querySelectorAll("a").forEach(function (link) {
            link.addEventListener("click", function () {
                links.classList.remove("open");
                toggle.setAttribute("aria-expanded", "false");
                toggle.setAttribute("aria-label", "Open navigation");
                toggle.textContent = "☰";
            });
        });

        // Close the menu when the user clicks outside the navigation.
        document.addEventListener("click", function (event) {
            const clickedInsideNav = links.contains(event.target);
            const clickedToggle = toggle.contains(event.target);

            if (!clickedInsideNav && !clickedToggle) {
                links.classList.remove("open");
                toggle.setAttribute("aria-expanded", "false");
                toggle.setAttribute("aria-label", "Open navigation");
                toggle.textContent = "☰";
            }
        });
    }

    // Automatically display the current year in the footer.
    document.querySelectorAll("[data-year]").forEach(function (element) {
        element.textContent = new Date().getFullYear();
    });

    // Contact form: prepare an email rather than submitting to a server.
    const form = document.querySelector("[data-contact-form]");

    if (form) {
        form.addEventListener("submit", function (event) {
            event.preventDefault();

            if (!form.reportValidity()) {
                return;
            }

            const name = form.querySelector('[name="name"]').value.trim();
            const email = form.querySelector('[name="email"]').value.trim();
            const topic = form.querySelector('[name="topic"]').value;
            const message = form.querySelector('[name="message"]').value.trim();

            const status = form.querySelector("[data-form-message]");

            // Change this to your verified, monitored support email.
            const supportEmail = "support@usedivity.com";

            const subject = encodeURIComponent(
                "Divity Contact: " + topic
            );

            const body = encodeURIComponent(
                "Name: " + name + "\n" +
                "Email: " + email + "\n" +
                "Topic: " + topic + "\n\n" +
                "Message:\n" + message
            );

            if (status) {
                status.textContent =
                    "Opening your email application. Your message has not " +
                    "been submitted to the website.";
            }

            window.location.href =
                "mailto:" + supportEmail +
                "?subject=" + subject +
                "&body=" + body;
        });
    }
});