module BoleroMultipageDemo.Client.Home

open System
open Elmish
open Bolero
open Bolero.Html
open Bolero.Remoting
open Bolero.Remoting.Client
open Bolero.Templating.Client

type Model = unit

let initModel = ()

type Msg = unit

let update remote message model =
    model, Cmd.none

type Home = Template<"wwwroot/home.html">

let view model dispatch =
    Home().Elt()