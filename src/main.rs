// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#![deny(warnings)]
#![allow(dead_code)]
#![allow(unused_variables)]
#![allow(unused_imports)]

mod api;
mod commands;
mod interface;
mod model;
mod old;
mod requests;
mod test_data;

use api::{command, Command, CommandGroup, CommandList, DisplayErrorCommand, Request};
use warp::Filter;

#[macro_use]
extern crate lazy_static;
extern crate eyre;

fn error_command(message: String) -> CommandList {
    CommandList {
        command_groups: vec![CommandGroup {
            commands: vec![Command {
                command: Some(command::Command::DisplayError(DisplayErrorCommand {
                    error: message,
                })),
            }],
        }],
    }
}

#[tokio::main]
async fn main() {
    let api = warp::post()
        .and(warp::path("api"))
        .and(warp::body::content_length_limit(1024 * 16))
        .and(warp_protobuf::body::protobuf())
        .map(|request: Request| {
            let mut description = format!("{:?}", request);
            description.truncate(500);
            println!("<----- Got request:\n{}", description);
            match requests::handle_request(request) {
                Ok(response) => {
                    let mut response_description = format!("{:?}", response);
                    response_description.truncate(500);
                    println!(
                        "-----> Sending {} command(s) in response:\n{}",
                        response.command_groups.len(),
                        response_description
                    );
                    warp_protobuf::reply::protobuf(&response)
                }
                Err(error) => {
                    let error = format!("Error! {:?}", error);
                    eprintln!("{}", error);
                    warp_protobuf::reply::protobuf(&error_command(error))
                }
            }
        });

    let site = warp::any().and(warp::fs::dir("static"));
    let routes = api.or(site);

    println!("Server started at http://localhost:3030");
    warp::serve(routes).run(([127, 0, 0, 1], 3030)).await;
}
