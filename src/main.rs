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

mod api;
mod old;

use warp::Filter;

#[macro_use]
extern crate lazy_static;

#[tokio::main]
async fn main() {
    let routes = warp::post()
        .and(warp::path("api"))
        .and(warp::body::content_length_limit(1024 * 16))
        .and(warp_protobuf::body::protobuf())
        .map(|influence: api::Influence| {
            println!(
                "Hello, influence {:?} of type {:?}",
                influence.influence_type(),
                influence.value
            );
            let response = api::Asset {
                address: String::from("Hello, asset"),
                asset_type: api::AssetType::Prefab.into(),
            };
            warp_protobuf::reply::protobuf(&response)
        });

    println!("Server started at http://localhost:3030");
    warp::serve(routes).run(([127, 0, 0, 1], 3030)).await;
}
