namespace PaymentWebhookController

open PaymentWebhookModel
open PaymentServiceConnector

module WebhookEventController =
    let validate (e: WebhookEvent.WebhookEvent) =
        if e.event = "" then false
        else if e.transaction_id = "" then false
        else if e.amount < 0.0 then false
        else if e.currency = "" then false
        else if e.timestamp = "" then false
        else true

    let handle (e: WebhookEvent.WebhookEvent, c: HttpConnector) =
        if validate e then
            c.ConfirmPayment e.transaction_id
            printfn "Payment confirmed for transaction %s" e.transaction_id
            true
        else
            c.CancelPayment e.transaction_id
            printfn "Payment cancelled for transaction %s" e.transaction_id
            false
