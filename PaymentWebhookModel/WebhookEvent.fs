namespace PaymentWebhookModel

open System

module WebhookEvent =
    type WebhookEvent = {
        event : string
        transaction_id : string
        amount : string
        currency : string
        timestamp : string
    }
