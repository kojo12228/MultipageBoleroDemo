module BoleroMultipageDemo.Client.Home

open System
open Elmish
open Bolero
open Bolero.Html
open Bolero.Json
open Bolero.Remoting
open Bolero.Remoting.Client
open Bolero.Templating.Client

type Model = unit

let initModel = ()

let init() = (), Cmd.none

type Msg = unit

type Home = Template<"wwwroot/home.html">

let view model dispatch =
    Home().Elt()