module BoleroMultipageDemo.Client.Main

open System
open Elmish
open Bolero
open Bolero.Html
open Bolero.Remoting
open Bolero.Remoting.Client
open Bolero.Templating.Client

open BoleroMultipageDemo.Client

/// Routing endpoints definition.
type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/counter">] Counter
    | [<EndPoint "/data">] Data

/// The Elmish application's model.
type Model =
    {
        page: Page
        homeModel: Home.Model
        counterModel: Counter.Model
        dataModel: Data.Model
    }

let initModel =
    {
        page = Home
        homeModel = Home.initModel
        counterModel = Counter.initModel
        dataModel = Data.initModel
    }

/// The Elmish application's update messages.
type Message =
    | SetPage of Page
    | HomeMessage of Home.Msg
    | CounterMessage of Counter.Msg
    | DataMessage of Data.Msg

let update remote message model =
    let onSignIn = function
        | Some _ -> Cmd.ofMsg (DataMessage Data.GetBooks)
        | None -> Cmd.none
    match message with
    | SetPage page ->
        { model with page = page }, Cmd.none
    | HomeMessage msg ->
        let homeModel, cmd = Home.update remote msg model.homeModel
        { model with homeModel = homeModel }, Cmd.map HomeMessage cmd
    | CounterMessage msg ->
        let counterModel, cmd = Counter.update remote msg model.counterModel
        { model with counterModel = counterModel }, Cmd.map CounterMessage cmd
    | DataMessage msg ->
        let dataModel, cmd = Data.update remote msg model.dataModel
        { model with dataModel = dataModel }, Cmd.map DataMessage cmd

/// Connects the routing system to the Elmish application.
let router = Router.infer SetPage (fun model -> model.page)

type Main = Template<"wwwroot/main.html">

let menuItem (model: Model) (page: Page) (text: string) =
    Main.MenuItem()
        .Active(if model.page = page then "is-active" else "")
        .Url(router.Link page)
        .Text(text)
        .Elt()

let view model dispatch =
    let mapDispatch msgWrapper = msgWrapper >> dispatch

    Main()
        .Menu(concat {
            menuItem model Home "Home"
            menuItem model Counter "Counter"
            menuItem model Data "Download data"
        })
        .Body(
            cond model.page <| function
            | Home ->
                Home.view model.homeModel (mapDispatch HomeMessage)
            | Counter ->
                Counter.view model.counterModel (mapDispatch CounterMessage)
            | Data ->
                Data.view model.dataModel (mapDispatch DataMessage)
        )
        .Elt()

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override this.Program =
        let bookService = this.Remote<Data.BookService>()
        let update = update bookService
        Program.mkProgram (fun _ -> initModel, Cmd.ofMsg (DataMessage Data.GetSignedInAs)) update view
        |> Program.withRouter router
#if DEBUG
        |> Program.withHotReload
#endif
