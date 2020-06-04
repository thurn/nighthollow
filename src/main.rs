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
#![allow(unused_imports)]
#![allow(unused_variables)]
#![allow(clippy::single_match)]
#![allow(clippy::unit_arg)]
#![feature(clamp)]
#![feature(move_ref_pattern)]
#![feature(map_first_last)]

mod api;
mod commands;
mod gameplay;
mod interface;
mod model;
mod prelude;
mod requests;
mod rules;
mod test_data;

use std::time::Instant;

use api::{command, Command, CommandGroup, CommandList, DisplayErrorCommand, Request};
use warp::Filter;

#[macro_use]
extern crate lazy_static;
extern crate eyre;

pub enum Empty {}

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
            let now = Instant::now();
            let result = requests::handle_request(&request);
            let completed = now.elapsed().as_secs_f64();
            match result {
                Ok(response) => {
                    let response_description = response
                        .command_groups
                        .iter()
                        .map(|group| {
                            group
                                .commands
                                .iter()
                                .map(commands::command_name)
                                .collect::<Vec<&str>>()
                        })
                        .collect::<Vec<Vec<&str>>>();
                    println!(
                        "-----> Sending {} groups(s) in {:.3} seconds\n{:?}",
                        response.command_groups.len(),
                        completed,
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
