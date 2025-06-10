namespace PaymentWebhookModel

open System

module WebhookEvent =
    type tEvent =
    | PaymentSuccess
    | PaymentFailure
    | RefundIssued
    
    type WebhookEvent = {
        event : string
        transaction_id : string
        amount : float
        currency : string
        timestamp : System.DateTime
    }

    let StringOfType e =
        match e with
        | PaymentSuccess -> "payment_success"
        | PaymentFailure -> "payment_failure"
        | RefundIssued -> "refund_issued"

    let TypeOfString s =
        match s with
        | "payment_success" -> PaymentSuccess
        | "payment_failure" -> PaymentFailure
        | "refund_issued" -> RefundIssued
        | _ -> failwith "Unknown event type: "
