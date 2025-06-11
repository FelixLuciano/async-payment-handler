namespace PaymentWebhookController

open PaymentWebhookModel
open PaymentServiceConnector

module WebhookEventController =
    let validateAmount (a: string) =
        match System.Decimal.TryParse(a) with
        | (true, v) when v > 0.00M -> true
        | _ -> false

    let validate (e: WebhookEvent.WebhookEvent) =
        if e.event = "" then false
        elif not (validateAmount e.amount) then false
        elif e.transaction_id = "" then false
        elif e.currency = "" then false
        elif e.timestamp = "" then false
        else true

    let handle (e: WebhookEvent.WebhookEvent, c: HttpConnector) =
        if validate e then
            c.ConfirmPayment e.transaction_id
            true
        else
            c.CancelPayment e.transaction_id
            false
